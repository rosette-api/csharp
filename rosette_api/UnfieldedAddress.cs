
namespace rosette_api
{
    public class UnfieldedAddress: IAddress
    {
        public UnfieldedAddress(string address = null)
        {
            this.address = address;
        }

        /// <summary>address
        /// <para>
        /// Getter, Setter for the address
        /// </para>
        /// </summary>
        public string address { get; set; }

        /// <summary> is this address fielded?
        /// </summary>
        public bool fielded()
        {
            return false;
        }
    }
}