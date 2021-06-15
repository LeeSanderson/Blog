using System;
using System.Collections.Generic;
using System.Text;
using Markdig.Helpers;
using Markdig.Parsers;
using Markdig.Renderers;
using Markdig.Renderers.Html;
using Markdig.Syntax;

namespace BlogToHtml.MarkdigExtensions.Prism
{
    internal class PrismCodeBlockRenderer : HtmlObjectRenderer<CodeBlock>
    {
        private static HashSet<string> _supportedLanguages = new(new[] {
            // ReSharper disable StringLiteralTypo
            "markup",
            "css", 
            "clike",
            "javascript",
            "abap",
            "actionscript",
            "ada",
            "apacheconf",
            "apl",
            "applescript",
            "arduino",
            "arff",
            "asciidoc",
            "asm6502",
            "aspnet",
            "autohotkey",
            "autoit",
            "bash",
            "basic",
            "batch",
            "bison",
            "brainfuck",
            "bro",
            "c",
            "csharp",
            "cpp",
            "coffeescript",
            "clojure",
            "crystal",
            "csp",
            "css-extras",
            "d",
            "dart",
            "diff",
            "django",
            "docker",
            "eiffel",
            "elixir",
            "elm",
            "erb",
            "erlang",
            "fsharp",
            "flow",
            "fortran",
            "gedcom",
            "gherkin",
            "git",
            "glsl",
            "gml",
            "go",
            "graphql",
            "groovy",
            "haml",
            "handlebars",
            "haskell",
            "haxe",
            "http",
            "hpkp",
            "hsts",
            "ichigojam",
            "icon",
            "inform7",
            "ini",
            "io",
            "j",
            "java",
            "jolie",
            "json",
            "julia",
            "keyman",
            "kotlin",
            "latex",
            "less",
            "liquid",
            "lisp",
            "livescript",
            "lolcode",
            "lua",
            "makefile",
            "markdown",
            "markup-templating",
            "matlab",
            "mel",
            "mizar",
            "monkey",
            "n4js",
            "nasm",
            "nginx",
            "nim",
            "nix",
            "nsis",
            "objectivec",
            "ocaml",
            "opencl",
            "oz",
            "parigp",
            "parser",
            "pascal",
            "perl",
            "php",
            "php-extras",
            "plsql",
            "powershell",
            "processing",
            "prolog",
            "properties",
            "protobuf",
            "pug",
            "puppet",
            "pure",
            "python",
            "q",
            "qore",
            "r",
            "jsx",
            "tsx",
            "renpy",
            "reason",
            "rest",
            "rip",
            "roboconf",
            "ruby",
            "rust",
            "sas",
            "sass",
            "scss",
            "scala",
            "scheme",
            "smalltalk",
            "smarty",
            "sql",
            "soy",
            "stylus",
            "swift",
            "tap",
            "tcl",
            "textile",
            "tt2",
            "twig",
            "typescript",
            "vbnet",
            "velocity",
            "verilog",
            "vhdl",
            "vim",
            "visual-basic",
            "wasm",
            "wiki",
            "xeora",
            "xojo",
            "xquery",
            "yaml"
            // ReSharper restore StringLiteralTypo
        });

        private readonly CodeBlockRenderer codeBlockRenderer;

        public PrismCodeBlockRenderer(CodeBlockRenderer? codeBlockRenderer) =>
            this.codeBlockRenderer = codeBlockRenderer ?? new CodeBlockRenderer();

        protected override void Write(HtmlRenderer renderer, CodeBlock node)
        {
            FencedCodeBlock? fencedCodeBlock = node as FencedCodeBlock;
            FencedCodeBlockParser? parser = node.Parser as FencedCodeBlockParser;
            if (fencedCodeBlock == null || parser == null)
            {
                codeBlockRenderer.Write(renderer, node);
                return;
            }

            string language = fencedCodeBlock.Info!.Replace(parser.InfoPrefix ?? string.Empty, string.Empty);
            if (string.IsNullOrWhiteSpace(language) || !_supportedLanguages.Contains(language))
            {
                codeBlockRenderer.Write(renderer, node);
                return;
            }

            var htmlAttributes = new HtmlAttributes();
            htmlAttributes.AddClass("language-" + language);
            string sourceCode = ExtractSourceCode((LeafBlock) node);
            renderer
                .Write("<pre>")
                    .Write("<code")
                        .WriteAttributes(htmlAttributes)
                        .Write(">")
                            .Write(sourceCode)
                    .Write("</code>")
                .Write("</pre>");
        }

        private static string ExtractSourceCode(LeafBlock node)
        {
            var stringBuilder = new StringBuilder();
            var lines = node.Lines.Lines;
            var length = lines.Length;
            for (var index = 0; index < length; ++index)
            {
                var slice = lines[index].Slice;
                if (slice.Text != null)
                {
                    string str = slice.Text.Substring(slice.Start, slice.Length);
                    if (index > 0)
                        stringBuilder.AppendLine();
                    stringBuilder.Append(str);
                }
            }
            return stringBuilder.ToString();
        }
    }
}