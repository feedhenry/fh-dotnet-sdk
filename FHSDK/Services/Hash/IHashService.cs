namespace FHSDK.Services.Hash
{
    /// <summary>
    /// Interface to provide a SHA1 hash method.
    /// </summary>
	public interface IHashService
	{
		string GenerateSha1Hash(string str);
	}


}

