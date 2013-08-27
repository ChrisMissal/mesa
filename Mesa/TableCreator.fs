namespace Mesa

open Mesa
open System
open System.Data
open System.Linq

type TableCreator() = 

    static member run (connection:string, filename:string) =
        System.Console.WriteLine("connection=" + connection)
        System.Console.WriteLine("filename=" + filename)

        let runSql(sql:string) =
            System.Console.WriteLine(sql)
            use connection = new System.Data.SqlClient.SqlConnection(connection)
            let txn = connection.Open()
            use command = connection.CreateCommand()
            command.CommandText <- sql
            command.CommandType <- CommandType.Text
            let count = command.ExecuteScalar()
            count

        let tableCreator(tableName:string, filename:string) =
            let parser = new Mesa.Parser()
            let rows = parser.records(',', filename)

            let sb = new System.Text.StringBuilder()
            sb.AppendLine(String.Format("create table {0} ( ", tableName))

            for row in rows.Take(1) do // assumed header
                for col in row do
                    sb.AppendLine(String.Format("{0} varchar(max), ", col))

            sb.Append(");")
            sb.ToString()

        let tablePopulateSql(tableName:string, filename:string) =
            let parser = new Mesa.Parser()
            let rows = parser.records(',', filename)
            let sb = new System.Text.StringBuilder()
            sb.AppendLine(String.Format("insert into {0} values (", tableName))
            for row in rows.Skip(1) do // skip header
                for col in row do
                    sb.Append(String.Format("'{0}',", col))

            sb.Append(");")
            sb.ToString().Replace(",)", ")") // wow that's gross, but i want this done for the evening

        let tableName = filename.Split('.').[0]

        let tableCreateSql = tableCreator(tableName, filename)
        runSql(tableCreateSql)

        let tablePopulateSql = tablePopulateSql(tableName, filename)
        runSql(tablePopulateSql)

    member this.X = "F#"
