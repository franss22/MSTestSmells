using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;
using System.ComponentModel.Composition;

namespace DTEtest
{
    /// <summary>
    /// Classification type definition export for DTETest
    /// </summary>
    internal static class DTETestClassificationDefinition
    {
        // This disables "The field is never used" compiler's warning. Justification: the field is used by MEF.
#pragma warning disable 169

        /// <summary>
        /// Defines the "DTETest" classification type.
        /// </summary>
        [Export(typeof(ClassificationTypeDefinition))]
        [Name("DTETest")]
        private static ClassificationTypeDefinition typeDefinition;

#pragma warning restore 169
    }
}
