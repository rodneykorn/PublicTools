using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckSign
{
    /// <summary>
    /// Certificate List Information
    /// </summary>
    class CertListInfo
    {
        /// <summary>
        /// Gets the list of data.
        /// </summary>
        /// <value>
        /// The list of data.
        /// </value>
        public List<string> Data { get; } = new List<string>();
        
        /// <summary>  
        /// Gets or sets the name of the certificate
        /// </summary>
        /// <value>
        /// The name of the Certificate.
        /// </value>
        public string Name { get; set; }

        /// <summary>Gets or sets a value indicating whether [valid for production].</summary>
        /// <value>
        ///   <c>true</c> if [valid for production]; otherwise, <c>false</c>.
        /// </value>
        public bool ValidForProd { get; set; }
    }
}
