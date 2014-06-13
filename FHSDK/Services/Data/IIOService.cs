using System;

namespace FHSDK
{
	public interface IIOService
	{
		string ReadFile(string fullPath);
		void WriteFile(string fullPath, string content);
		bool Exists(string fullPath);
		string GetDataPersistDir();
	}
}

