using System.IO;
using System.Security.Cryptography;
using System.Text;

public static class MD5
{
	private static uint CalcMD5(byte[] bytes)
	{
		return (uint) (
			(bytes[0] & 0xFF)
			| ((bytes[1] & 0xFF) << 8)
			| ((bytes[2] & 0xFF) << 16)
			| ((bytes[3] & 0xFF) << 24)
		);
	}

	public static uint GetMD5FromString(string data)
	{
		var md5 = new MD5CryptoServiceProvider();
		var bytes = md5.ComputeHash(Encoding.Default.GetBytes(data));
		return CalcMD5(bytes);
	}

	public static uint GetMD5FromFile(string path)
	{
		var file = new FileStream(path, FileMode.Open);
		var md5 = new MD5CryptoServiceProvider();
		var bytes = md5.ComputeHash(file);
		file.Close();
		return CalcMD5(bytes);
	}
}