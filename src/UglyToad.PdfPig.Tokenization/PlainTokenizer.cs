﻿namespace UglyToad.PdfPig.Tokenization
{
    using Core;
    using System.Text;
    using Tokens;

    internal class PlainTokenizer : ITokenizer
    {
        private readonly StringBuilder stringBuilder = new();

        public bool ReadsNextByte => true;

        public bool TryTokenize(byte currentByte, IInputBytes inputBytes, out IToken token)
        {
            token = null;

            if (ReadHelper.IsWhitespace(currentByte))
            {
                return false;
            }

            var builder = stringBuilder;
            builder.Append((char)currentByte);
            while (inputBytes.MoveNext())
            {
                if (ReadHelper.IsWhitespace(inputBytes.CurrentByte))
                {
                    break;
                }

                if (inputBytes.CurrentByte == '<' || inputBytes.CurrentByte == '['
                    || inputBytes.CurrentByte == '/' || inputBytes.CurrentByte == ']'
                    || inputBytes.CurrentByte == '>' || inputBytes.CurrentByte == '('
                    || inputBytes.CurrentByte == ')')
                {
                    break;
                }

                builder.Append((char) inputBytes.CurrentByte);
            }

            var text = builder.ToString();
            builder.Clear();

            switch (text)
            {
                case "true":
                    token = BooleanToken.True;
                    break;
                case "false":
                    token = BooleanToken.False;
                    break;
                case "null":
                    token = NullToken.Instance;
                    break;
                default:
                    token = OperatorToken.Create(text);
                    break;
            }

            return true;
        }
    }
}
