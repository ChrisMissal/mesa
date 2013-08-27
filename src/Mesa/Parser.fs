namespace Mesa

open System
open System.IO

// All credit here: http://brianary.blogspot.com/2011/04/f-mutually-tail-recursive-csv-record.html
type Parser() =

    // An F# mutually-tail-recursive CSV record parser.
    // See the spec at http://creativyst.com/Doc/Articles/CSV/CSV01.htm
    let rec csvrecord sep (tr:#TextReader) record (line:string) i =
      if i = line.Length then record @ [""]
      else
          match line.[i] with
          | '"' -> csvfield sep tr record "" line (i+1)
          | ' ' | '\t'-> csvrecord sep tr record line (i+1)
          |  c  when c = sep -> csvrecord sep tr (record @ [""]) line (i+1)
          | '=' when line.[i+1] = '"' -> csvfield sep tr record "" line (i+2) // Excel compatibility
          | _ -> // unquoted field data
              let fs = line.IndexOf(sep,i)
              if fs = -1 then record @ [line.Substring(i).TrimEnd()]
              else
                  csvrecord sep tr (record @ [line.Substring(i,fs-i).TrimEnd()]) line (fs+1)
    and csvfield sep (tr:#TextReader) record field (line:string) i =
      if i = line.Length then csvfield sep tr record (field+"\n") (tr.ReadLine()) 0
      elif line.[i] <> '"' then
          let q = line.IndexOf('"',i)
          if q = -1 then csvfield sep tr record (field+line.Substring(i)+"\n") (tr.ReadLine()) 0
          else csvfield sep tr record (field+line.Substring(i,q-i)) line q
      elif i = line.Length-1 then record @ [field]
      elif line.[i+1] = '"' then csvfield sep tr record (field+"\"") line (i+2)
      elif line.[i+1] = sep then csvrecord sep tr (record @ [field]) line (i+2)
      else // not an escaped quote and not end of field; try to recover by appending trimmed unquoted field data
          let fs = line.IndexOf(sep,i+1)
          if fs = -1 then record @ [field+line.Substring(i).TrimEnd()]
          else csvrecord sep tr (record @ [field+line.Substring(i,fs-i).TrimEnd()]) line (fs+1)

    let csvrows sep (filepath:string) = seq {
      use sr = new StreamReader(filepath)
      while not sr.EndOfStream do
          yield csvrecord sep sr [] (sr.ReadLine()) 0
    }

    let csvrecords sep (filepath:string) = seq {
      let lists = csvrows sep filepath
      let headers = Seq.head lists
      for vals in (Seq.skip 1 lists) do
          yield List.zip headers vals |> Map.ofList
    }

    member this.records(seperator:char, filename:string) = 
        csvrows seperator filename
