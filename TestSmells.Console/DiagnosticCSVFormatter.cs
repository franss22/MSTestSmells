using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis;

using System.Globalization;

using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace TestSmells.Console
{
    public class DiagnosticCSVFormatter
    {
        /// <summary>
        /// Formats the <see cref="Diagnostic"/> message using the optional <see cref="IFormatProvider"/>.
        /// </summary>
        /// <param name="diagnostic">The diagnostic.</param>
        /// <param name="formatter">The formatter; or null to use the default formatter.</param>
        /// <returns>The formatted message.</returns>
        public virtual string Format(Diagnostic diagnostic, IFormatProvider? formatter = null)
        {
            return Format(diagnostic, diagnostic.Severity, formatter);
        }


        public virtual string Format(Diagnostic diagnostic, DiagnosticSeverity severity, IFormatProvider? formatter = null)
        {
            if (diagnostic == null)
            {
                throw new ArgumentNullException(nameof(diagnostic));
            }

            var culture = formatter as CultureInfo;

            switch (diagnostic.Location.Kind)
            {
                case LocationKind.SourceFile:
                case LocationKind.XmlFile:
                case LocationKind.ExternalFile:
                    var span = diagnostic.Location.GetLineSpan();
                    var mappedSpan = diagnostic.Location.GetMappedLineSpan();
                    if (!span.IsValid || !mappedSpan.IsValid)
                    {
                        goto default;
                    }

                    string? path, basePath;
                    if (mappedSpan.HasMappedPath)
                    {
                        path = mappedSpan.Path;
                        basePath = span.Path;
                    }
                    else
                    {
                        path = span.Path;
                        basePath = null;
                    }

                    return string.Format(formatter, "{0}, {1}, {2}, {3}{4}",
                                         FormatSourcePath(path, basePath, formatter),
                                         FormatSourceSpan(mappedSpan.Span, formatter),
                                         GetMessagePrefix(diagnostic, severity),
                                         diagnostic.GetMessage(culture),
                                         FormatHelpLinkUri(diagnostic));

                default:
                    var prefix = GetMessagePrefix(diagnostic, severity);
                    var message = diagnostic.GetMessage(culture);
                    var helplink = FormatHelpLinkUri(diagnostic);

                    return string.Format(formatter, "{0}, {1}{2}", prefix, message, helplink);

                    
            }
        }

        internal virtual string FormatSourcePath(string path, string? basePath, IFormatProvider? formatter)
        {
            // ignore base path
            return path;
        }

        internal virtual string FormatSourceSpan(LinePositionSpan span, IFormatProvider? formatter)
        {
            return string.Format("{0}, {1}", span.Start.Line + 1, span.Start.Character + 1);
        }

        internal static string GetMessagePrefix(Diagnostic diagnostic, DiagnosticSeverity severity)
        {
            string prefix;
            switch (severity)
            {
                case DiagnosticSeverity.Hidden:
                    prefix = "hidden";
                    break;
                case DiagnosticSeverity.Info:
                    prefix = "info";
                    break;
                case DiagnosticSeverity.Warning:
                    prefix = "warning";
                    break;
                case DiagnosticSeverity.Error:
                    prefix = "error";
                    break;
                default:
                    throw ExceptionUtilities.UnexpectedValue(severity);
            }

            return string.Format("{0}, {1}", prefix, diagnostic.Id);
        }

        private string FormatHelpLinkUri(Diagnostic diagnostic)
        {
            var uri = diagnostic.Descriptor.HelpLinkUri;

            if (string.IsNullOrEmpty(uri) || HasDefaultHelpLinkUri(diagnostic))
            {
                return string.Empty;
            }

            return $", {uri}";
        }

        internal virtual bool HasDefaultHelpLinkUri(Diagnostic diagnostic) => true;

        internal static readonly DiagnosticCSVFormatter Instance = new();
    }

    internal static class ExceptionUtilities
    {
        /// <summary>
        /// Creates an <see cref="InvalidOperationException"/> with information about an unexpected value.
        /// </summary>
        /// <param name="o">The unexpected value.</param>
        /// <returns>The <see cref="InvalidOperationException"/>, which should be thrown by the caller.</returns>
        internal static Exception UnexpectedValue(object? o)
        {
            string output = string.Format("Unexpected value '{0}' of type '{1}'", o, (o != null) ? o.GetType().FullName : "<unknown>");
            Debug.Assert(false, output);

            // We do not throw from here because we don't want all Watson reports to be bucketed to this call.
            return new InvalidOperationException(output);
        }

        internal static Exception Unreachable([CallerFilePath] string? path = null, [CallerLineNumber] int line = 0)
            => new InvalidOperationException($"This program location is thought to be unreachable. File='{path}' Line={line}");

        /// <summary>
        /// Determine if an exception was an <see cref="OperationCanceledException"/>, and that the provided token caused the cancellation.
        /// </summary>
        /// <param name="exception">The exception to test.</param>
        /// <param name="cancellationToken">Checked to see if the provided token was cancelled.</param>
        /// <returns><see langword="true"/> if the exception was an <see cref="OperationCanceledException" /> and the token was canceled.</returns>
        internal static bool IsCurrentOperationBeingCancelled(Exception exception, CancellationToken cancellationToken)
            => exception is OperationCanceledException && cancellationToken.IsCancellationRequested;
    }
}
