using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Wallpaperomatic
{
    public class ParseCommandLine
    {
        private readonly Dictionary<string, List<string>> _arguments;
        public ParseCommandLine(string commandLine)
        {
            const string pattern = "(/|-{1,2})(?<key>[^\\s:=]+)([\\s:=]+(\"(?<value>[^\"]*)\"|(?<value>[^\\s/-]*)))?";
            var matches = Regex.Matches(commandLine, pattern);

            _arguments = new Dictionary<string, List<string>>();
            foreach (Match match in matches)
            {
                var key = match.Groups["key"].Value.ToLower();
                var value = match.Groups["value"].Value;

                if (!_arguments.ContainsKey(key))
                {
                    _arguments.Add(key, new List<string>());
                }
                if (!string.IsNullOrEmpty(value))
                {
                    _arguments[key].Add(value);
                }
            }
        }

        public IList<string> this[string key]
        {
            get
            {
                return (_arguments[key.ToLower()]);
            }
        }

        public T GetFirstOrSubstitute<T>(string key, T substituteValue)
        {
            if (!ContainsKey(key)) return substituteValue;

            var value = this[key];

            if (value.Count > 0)
            {
                try
                {
                    return (T)Convert.ChangeType(value[0], typeof(T));
                }
                catch { }
            }
            
            return substituteValue;
        }

        public bool ContainsKey(string key)
        {
            return _arguments.ContainsKey(key.ToLower());
        }
    }
}
