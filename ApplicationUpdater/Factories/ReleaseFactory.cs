using System;
using System.Linq;

namespace ApplicationUpdater
{
    internal static class ReleaseFactory
    {
        public static Release CreateReleaseEntry(string release)
        {
            var properties = release.Split(new [] { ":" }, StringSplitOptions.RemoveEmptyEntries)
                                    .Where(p => !string.IsNullOrWhiteSpace(p))
                                    .Select(p => p.Trim())
                                    .ToList();
            if (properties.Count < 2)
            {
                // TODO: May ot be the right behavior for a Factory method
                throw new ArgumentException(@"Release format is <filename> : <version> : [comment]", "release");
            }

            return new Release(properties[0], properties[1]);
        }
    }
}