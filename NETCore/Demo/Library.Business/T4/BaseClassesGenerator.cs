using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Demo.Library.Business.T4
{
    public static class BaseClassesGenerator
    {
        public static string DoWork(string path)
        {
            var files = Directory.GetFiles(path, "*Model.cs", SearchOption.AllDirectories).ToList();

            files.ForEach(file =>
            {
                ProcessFile(file);
            });

            return string.Join(Environment.NewLine, files);
        }

        public static void ProcessFile(string fileName)
        {
            var all = File.ReadAllLines(fileName);
            var res = new List<string>();
            var index = 0;
            var modelClassName = "";
            var className = "";
            var nameSpace = "";

            res.Add("using AutoMapper;");
            res.Add("using PROTR.Core;");

            for (var lineIndex = 0; lineIndex < all.Length; ++lineIndex)
            {
                var line = all[lineIndex];
                var parts = line.Trim().Split(' ');

                if (line.Contains("namespace"))
                {
                    nameSpace = parts[1].Replace(".EF", "");
                }

                if (line.Contains("public"))
                {
                    var type = parts[1];

                    if (type == "class" && parts.Length > 3)
                    {
                        return;
                    }

                    if (type == "class")
                    {
                        var profileClassName = parts[2].Replace("Model", "Profile");

                        className = parts[2].Replace("Model", "");
                        modelClassName = parts[2];
                        lineIndex++;
                        res.Add(line.Replace("Model", "Base") + " : BusinessBase");
                        res.Add(@"    {
        public class " + profileClassName + @" : Profile
        {
            public " + profileClassName + @"()
            {
                CreateMap<" + modelClassName + @", " + className + @">();
                CreateMap<" + className + @", " + modelClassName + @">();
            }
        }

        public " + className + @"Base(ContextProvider contextProvider) : base(contextProvider)
        {
            ModelType = typeof(" + modelClassName + @");
        }
");
                    }
                    else if (type == "string")
                    {
                        res.Add(line.Replace("{ get; set; }", @"
        {
            get
            {
                return this[" + index + @"].ToString();
            }
            set
            {
                this[" + index + @"] = value;
            }
        }"));
                        index++;
                    }
                }
                else
                {
                    res.Add(line);
                }
            }

            var newFile = fileName.Replace("Model.cs", "Base.cs");
            if (File.Exists(newFile))
            {
                File.Delete(newFile);
            }
            File.WriteAllLines(newFile, res);

            var classFile = newFile.Replace("\\EF", "").Replace("Base", "");
            if (!File.Exists(classFile))
            {
                File.WriteAllText(classFile, @"using System;
using System.Collections.Generic;
using System.Text;
using " + nameSpace + @".EF;
using PROTR.Core;

namespace " + nameSpace + @"
{
    public class " + className + @" : " + className + @"Base
    {
        public " + className + @"(ContextProvider contextProvider) : base(contextProvider)
        {
        }
    }
}
");
            }
        }
    }
}
