namespace Valve.Sockets
{
    public enum EUserHasLicenseForAppResult : uint
    {
        k_EUserHasLicenseResultHasLicense = 0,
        k_EUserHasLicenseResultDoesNotHaveLicense = 1,
        k_EUserHasLicenseResultNoAuth = 2,
    }
}