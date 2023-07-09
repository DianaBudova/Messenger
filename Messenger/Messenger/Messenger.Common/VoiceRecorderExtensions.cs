namespace Messenger.Common;

public static class VoiceRecorderExtensions
{
    private const string folderName = "VoiceRecords";
    private const string pathName = "vr";
    private const string pathExtension = "wav";

    public static string? GenerateOutputPath()
    {
        try
        {
            if (!Directory.Exists(folderName))
                Directory.CreateDirectory(folderName);

            var voiceRecords = Directory.GetFiles(folderName);
            if (voiceRecords.Length <= 0)
                return $@"{folderName}\{pathName}0.{pathExtension}";

            Array.Sort(voiceRecords, CompareStringsByNumber);
            int nth = int.Parse(voiceRecords.Last().ToString().Where(ch => char.IsDigit(ch)).ToArray()) + 1;
            return $@"{folderName}\{pathName}{nth}.{pathExtension}";
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    private static int CompareStringsByNumber(string left, string right)
    {
        try
        {
            int leftNumber = int.Parse(left.Where(ch => char.IsDigit(ch)).ToArray());
            int rightNumber = int.Parse(right.Where(ch => char.IsDigit(ch)).ToArray());
            return leftNumber.CompareTo(rightNumber);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
}