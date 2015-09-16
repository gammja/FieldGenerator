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
            var targetClass = new CodeTypeDeclaration("CodeDOMCreatedClass");
            targetClass.IsClass = true;
            targetClass.TypeAttributes = TypeAttributes.Public | TypeAttributes.Sealed;

            var targetUnit = new CodeCompileUnit();
            var ns = GenerateNameSpace(targetClass);
            targetUnit.Namespaces.Add(ns);

            var field = GenerateField("_width", typeof(double));
            targetClass.Members.Add(field);

            var property = GenerateProperty(field);
            targetClass.Members.Add(property);

            GenerateCSharpCode(targetUnit, "SampleCode.cs");
        }

        private static CodeNamespace GenerateNameSpace(CodeTypeDeclaration targetClass)
        {
            var ns = new CodeNamespace("CodeDOMSample");
            ns.Imports.Add(new CodeNamespaceImport("System"));
            ns.Types.Add(targetClass);
            return ns;
        }

        private static CodeMemberField GenerateField(string name, Type type)
        {
            CodeMemberField widthValueField = new CodeMemberField();
            widthValueField.Attributes = MemberAttributes.Private;
            widthValueField.Name = name;
            widthValueField.Type = new CodeTypeReference(type);
            return widthValueField;
        }

        private static CodeTypeMember GenerateProperty(CodeMemberField field)
        {
            CodeMemberProperty widthProperty = new CodeMemberProperty();
            widthProperty.Attributes = MemberAttributes.Public | MemberAttributes.Final;
            widthProperty.Name = GetName(field);
            widthProperty.HasGet = true;
            widthProperty.Type = field.Type;
            widthProperty.GetStatements.Add(new CodeMethodReturnStatement(
                new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), field.Name)));
            return widthProperty;
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

        private static void GenerateCSharpCode(CodeCompileUnit targetUnit, string fileName)
        {
            CodeDomProvider provider = CodeDomProvider.CreateProvider("CSharp");
            CodeGeneratorOptions options = new CodeGeneratorOptions();
            options.BracingStyle = "C";
            using (StreamWriter sourceWriter = new StreamWriter(fileName))
            {
                provider.GenerateCodeFromCompileUnit(targetUnit, sourceWriter, options);
            }
        }
    }
}