
namespace rosette_api
{
    public class UnfieldedAddress : IAddress
    {
        public UnfieldedAddress(string? address = null)
        {
            this.Address = address;
        }

        /// <summary>address
        /// <para>
        /// Getter, Setter for the address
        /// </para>
        /// </summary>
        public string? Address { get; set; }

        /// <summary> is this address fielded?
        /// </summary>
        public bool Fielded()
        {
            return false;
        }
    }
}