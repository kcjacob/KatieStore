namespace KatieStore
{
    public interface IAddressValidationService
    {
        CheckedAddress[] CheckAddress(string street1, string street2, string city, string state, string postalCode);
    }
}