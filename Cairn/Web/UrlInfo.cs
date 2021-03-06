﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Cairn.Web {
    public class UrlInfo {

        private const string DefaultRegexSpec = @"
(?<protocol>http|ftp|https|ftps)://
(?<domain>[^/\r\n\?]+)
(?<file_path>
    (?<app_path>/[^\r\n\?]*)*
    /
    (?<file>
            (?<resource>[^\#\r\n\?]+(!?\.))
            \.
            (?<extension>[^\#\r\n\?\./]+)
    )
)?
(?<extensionless_path>/[^\r\n\?]*)?
(?:\?)?
(?<query>[^\#\r\n]*)?
(?<anchor>\#[^\#\r\n\?]*)?
";

        public static readonly Regex DefaultRegex = new Regex(DefaultRegexSpec,
                                                              RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace |
                                                              RegexOptions.Compiled);

        private readonly string _url;
        private readonly Regex _regex;
        private readonly Dictionary<string, string> _query;
        private Match _match;

        /// <summary>
        /// The regular expression being used to break URLs
        /// down into their parts.
        /// </summary>
        /// <remarks>
        /// If a regular expression isn't provided via the contructor
        /// then the default <see cref="UrlInfo.DefaultRegex"/> is used.
        /// </remarks>
        public Regex Regex {
            get { return _regex ?? UrlInfo.DefaultRegex; }
        }

        /// <summary>
        /// The matches produced by matching the
        /// <see cref="Regex"/> against the <see cref="Url"/>.
        /// </summary>
        public Match Match {
            get {
                if (_match == null) {
                    this.ProcessUrl();
                }
                return _match;
            }
        }

        /// <summary>
        /// The url being processed.
        /// </summary>
        public string Url {
            get { return _url; }
        }

        /// <summary>
        /// The protocol specified by the <see cref="Url"/>
        /// </summary>
        public string Protocol {
            get {
                return this.Match.Groups["protocol"].Value;
            }
        }

        /// <summary>
        /// The domain specified by the <see cref="Url"/>
        /// </summary>
        public string Domain {
            get {
                return this.Match.Groups["domain"].Value;
            }
        }

        /// <summary>
        /// The full path specified by the <see cref="Url"/>
        /// </summary>
        public string FullPath {
            get {
                if (IsFileResource)
                    return this.Match.Groups["file_path"].Value;
                else
                    return this.Match.Groups["extensionless_path"].Value;
            }
        }

        public bool IsFileResource {
            get {
                return !String.IsNullOrEmpty(this.Match.Groups["file_path"].Value);
            }
        }

        /// <summary>
        /// The URL of the current application.
        /// </summary>
        public string AppUrl {
            get {
                return String.Concat(this.Protocol, "://", this.Domain, this.AppPath);
            }
        }

        /// <summary>
        /// The application path specified by the <see cref="Url"/>
        /// </summary>
        public string AppPath {
            get {
                if (IsFileResource)
                    return this.Match.Groups["app_path"].Value;
                else
                    return this.Match.Groups["extensionless_path"].Value;
            }
        }

        /// <summary>
        /// The file specified by the <see cref="Url"/>
        /// </summary>
        public string File {
            get {
                return this.Match.Groups["file"].Value;
            }
        }

        /// <summary>
        /// The extension specified by the <see cref="Url"/>
        /// </summary>
        public string Extension {
            get {
                return this.Match.Groups["extension"].Value;
            }
        }

        /// <summary>
        /// The query string specified by the <see cref="Url"/>
        /// </summary>

        public string QueryString {
            get {
                return this.Match.Groups["query"].Value;
            }
        }

        /// <summary>
        /// A name / value dictionary as the propduct of
        /// parsing the <see cref="QueryString"/>
        /// </summary>
        public Dictionary<string, string> Query {
            get {
                return _query;
            }
        }

        public UrlInfo(Uri uri) : this(uri, null) { }
        public UrlInfo(string url) : this(url, null) { }
        public UrlInfo(Uri uri, Regex regex) : this(uri.ToString(), regex) { }

        public UrlInfo(string url, Regex regex) {
            _url = url;
            _regex = regex;
        }

        public UrlInfo(UrlInfo info) {
            _url = info.Url;
            _regex = info.Regex;
            _query = new Dictionary<string, string>();
        }

        public void ProcessUrl() {
            _match = this.Regex.Match(_url);
            if (!String.IsNullOrWhiteSpace(this.QueryString)) {

            }
        }
    }

}
