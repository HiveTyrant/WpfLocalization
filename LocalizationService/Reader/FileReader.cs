using System.IO;
using LocalizationService.Localization;

namespace LocalizationService.Reader
{
    public abstract class FileReader<T> 
    {

        #region Fields

        protected string Path;

        #endregion

        protected FileReader(string path)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException($"File '{path}' not found.", path);

            Path = path;
        }

        #region Abstract Methods

        internal abstract T GetEntries();

        #endregion

    }
}
