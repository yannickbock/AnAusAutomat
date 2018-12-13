using AnAusAutomat.Toolbox.Extensions;
using System;
using System.Collections.Generic;
using Xunit;

namespace AnAusAutomat.Toolbox.Tests.Extensions
{
    public class DictionaryExtensionsTests
    {
        [Fact]
        public void GetValueOrDefault()
        {
            var dictionary = new Dictionary<string, int>()
            {
                { "One", 1 },
                { "Two", 2 },
            };

            Assert.Equal(3, dictionary.GetValueOrDefault("Three", 3));
        }

        [Fact]
        public void GetValueOrDefault_DictionaryIsNull()
        {
            Dictionary<string, int> dictionary = null;

            Assert.Throws<ArgumentNullException>(() => { dictionary.GetValueOrDefault("Three", 3); });
        }

        [Fact]
        public void GetValueOrDefault_KeyIsNull()
        {
            var dictionary = new Dictionary<string, int>()
            {
                { "One", 1 },
                { "Two", 2 },
            };

            Assert.Throws<ArgumentNullException>(() => { dictionary.GetValueOrDefault(null, 3); });
        }
    }
}
