using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.IO;
using System.Reflection;

namespace FieldGenerator
{
    static class Sample
    {
        static void Main()
        {
            var className = "SpDataRow";
            var cls = new CodeTypeDeclaration(className);
            cls.IsClass = true;
            cls.IsPartial = true;
            cls.TypeAttributes = TypeAttributes.Public;

            var unit = new CodeCompileUnit();

            var nsName = "FieldGenerator";
            var ns = new CodeNamespace(nsName);
            ns.Imports.Add(new CodeNamespaceImport("System"));
            ns.Types.Add(cls);
            unit.Namespaces.Add(ns);

            var field = GenerateField("_age", typeof(double));
            cls.Members.Add(field);

            var property = GenerateProperty(field);
            cls.Members.Add(property);

            GenerateCSharpCode(unit, "SpDataRow.cs");
        }

        private static CodeMemberField GenerateField(string name, Type type)
        {
            CodeMemberField field = new CodeMemberField();
            field.Attributes = MemberAttributes.Private;
            field.Name = name;
            field.Type = new CodeTypeReference(type);
            return field;
        }

        private static CodeTypeMember GenerateProperty(CodeMemberField field)
        {
            CodeMemberProperty property = new CodeMemberProperty();
            property.Attributes = MemberAttributes.Public | MemberAttributes.Final;
            property.Name = GetName(field);
            property.Type = field.Type;

            property.HasGet = true;
            property.GetStatements.Add(new CodeMethodReturnStatement(
                new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), field.Name)));

            property.HasSet = true;
            property.SetStatements.Add(
                new CodeAssignStatement(
                    new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), field.Name),
                    new CodePropertySetValueReferenceExpression()));

            return property;
        }

        private static string GetName(CodeMemberField field)
        {
            var name = field.Name;
            if (string.IsNullOrWhiteSpace(name)) return string.Empty;

            if (name.StartsWith("_")) name = name.Substring(1);
            if (name.Length == 1) return name.ToUpper();

            // to title case
            return char.ToUpper(name[0]) + name.Substring(1);
        }

        private static void GenerateCSharpCode(CodeCompileUnit unit, string fileName)
        {
            var provider = CodeDomProvider.CreateProvider("CSharp");
            var options = new CodeGeneratorOptions {BracingStyle = "C"};
            using (var writer = new StreamWriter(fileName))
            {
                provider.GenerateCodeFromCompileUnit(unit, writer, options);
            }
        }
    }
}