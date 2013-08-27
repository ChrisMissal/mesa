module TestConventions

    type MainConvention() =
        inherit Fixie.Conventions.DefaultConvention()
        //new() =
            //base.Classes.NameEndsWith("Tests")
            //base.Cases.Where(fun m -> m.Method.Void())
