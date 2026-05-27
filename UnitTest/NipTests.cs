using System;
using AppCore.Models;
using Xunit;

namespace AppCore.Tests
{
    
    
    public class NIPTests
    {
        public class IsValid_CyfraKontrolna
        {
            [Theory]
            [InlineData("1010000002")]
            [InlineData("5260000005")]
            [InlineData("1230000004")]
            [InlineData("2010000008")]
            [InlineData("9010000006")]
            public void IsValid_PoprawnyNIP_ZwracaTrue(string nip)
            {
                Assert.True(NIP.IsValid(nip),
                    $"NIP '{nip}' powinien być poprawny, ale IsValid zwróciło false.");
            }

            [Theory]
            [InlineData("1010000001")]
            [InlineData("1010000003")]
            [InlineData("5260000009")]
            [InlineData("2010000001")]
            public void IsValid_BlednaOstatniaCyfra_ZwracaFalse(string nip)
            {
                Assert.False(NIP.IsValid(nip),
                    $"NIP '{nip}' ma błędną cyfrę kontrolną — IsValid powinno zwrócić false.");
            }

            [Fact]
            public void IsValid_SumaKontrolnaRowna10_ZwracaFalse()
            {

                Assert.False(NIP.IsValid("6010000000"),
                    "NIP z sumą kontrolną = 10 powinien być odrzucony.");
            }

            [Fact]
            public void IsValid_ZaKrotki_ZwracaFalse()
                => Assert.False(NIP.IsValid("101000000"), "9 cyfr — za krótki.");

            [Fact]
            public void IsValid_ZaDlugi_ZwracaFalse()
                => Assert.False(NIP.IsValid("10100000020"), "11 cyfr — za długi.");

            [Fact]
            public void IsValid_PustyString_ZwracaFalse()
                => Assert.False(NIP.IsValid(string.Empty));

            [Fact]
            public void IsValid_Null_ZwracaFalse()
                => Assert.False(NIP.IsValid(null!));

            [Theory]
            [InlineData("ABCDEFGHIJ")]
            [InlineData("123456789X")]
            public void IsValid_ZnakiNienumeryczne_ZwracaFalse(string nip)
                => Assert.False(NIP.IsValid(nip));
        }

        public class Sanitize_Tests
        {
            [Theory]
            [InlineData("101-000-00-02", "1010000002")]
            [InlineData("101 000 00 02", "1010000002")]
            [InlineData("101.000.00.02", "1010000002")]
            [InlineData("(101)000-0002", "1010000002")]
            [InlineData("  1010000002  ", "1010000002")]
            public void Sanitize_UsuwaNienumeryczneZnaki(string wejscie, string oczekiwane)
                => Assert.Equal(oczekiwane, NIP.Sanitize(wejscie));

            [Fact]
            public void Sanitize_Null_ZwracaPustyString()
                => Assert.Equal(string.Empty, NIP.Sanitize(null!));

            [Fact]
            public void Konstruktor_AkceptujeNIPZMyslnikami()
            {
                var nip = new NIP("101-000-00-02");
                Assert.Equal("1010000002", nip.Value);
            }

            [Fact]
            public void Konstruktor_AkceptujeNIPZeSpacjami()
            {
                var nip = new NIP("101 000 00 02");
                Assert.Equal("1010000002", nip.Value);
            }
        }

        public class GetTaxOfficeNumber_Tests
        {
            [Theory]
            [InlineData("1010000002", "101")]
            [InlineData("1020000009", "102")]
            [InlineData("2010000008", "201")]
            [InlineData("3010000003", "301")]
            [InlineData("6010000016", "601")]
            [InlineData("7010000005", "701")]
            [InlineData("8010000000", "801")]
            [InlineData("9010000006", "901")]
            public void GetTaxOfficeNumber_ZwracaPierwszeTrzyCyfry(string nipValue, string oczekiwanyPrefiks)
            {
                var nip = new NIP(nipValue);
                Assert.Equal(oczekiwanyPrefiks, nip.GetTaxOfficeNumber());
            }

            [Fact]
            public void GetTaxOfficeNumber_MaDlugosc3()
            {
                var nip = new NIP("1010000002");
                Assert.Equal(3, nip.GetTaxOfficeNumber().Length);
            }

            [Fact]
            public void GetTaxOfficeNumber_ZawieraWylacznieCyfry()
            {
                var nip = new NIP("1010000002");
                Assert.All(nip.GetTaxOfficeNumber(), c => Assert.True(char.IsDigit(c)));
            }
        }

        public class GetTaxOfficeName_Tests
        {
            [Theory]

            [InlineData("1010000002", "Pierwszy Urząd Skarbowy w Warszawie")]
            [InlineData("1020000009", "Drugi Urząd Skarbowy w Warszawie")]
            [InlineData("1030000005", "Urząd Skarbowy Warszawa-Mokotów")]

            [InlineData("2010000008", "Pierwszy Urząd Skarbowy w Krakowie")]
            [InlineData("2020000004", "Drugi Urząd Skarbowy w Krakowie")]
            public void GetTaxOfficeName_ZnanyPrefiks_ZwracaDokladnaNazwe(
                string nipValue, string oczekiwanaNazwa)
            {
                var nip = new NIP(nipValue);
                Assert.Equal(oczekiwanaNazwa, nip.GetTaxOfficeName());
            }

            [Theory]
            [InlineData("3010000003", "301")]
            [InlineData("6010000016", "601")]
            [InlineData("7010000005", "701")]
            [InlineData("8010000000", "801")]
            [InlineData("9010000006", "901")]
            public void GetTaxOfficeName_PrefixSpozaSlownika_ZwracaNieznanego(
                string nipValue, string prefiks)
            {
                var nip = new NIP(nipValue);
                var nazwa = nip.GetTaxOfficeName();
                Assert.Contains("Nieznany", nazwa, StringComparison.OrdinalIgnoreCase);
                Assert.Contains(prefiks, nazwa);
            }

            [Fact]
            public void GetTaxOfficeName_NieznanypPrefiks_ZawieraKomunikatZPrefiksem()
            {

                var nip = new NIP("9990000008");
                var nazwa = nip.GetTaxOfficeName();

                Assert.Contains("Nieznany", nazwa, StringComparison.OrdinalIgnoreCase);
                Assert.Contains("999", nazwa);
            }

            [Fact]
            public void GetTaxOfficeName_KazdeWynik_ZawieraSlowa_UrzadSkarbowy()
            {
                var znany    = new NIP("1010000002");
                var nieznany = new NIP("9990000008");

                Assert.Contains("Urząd Skarbowy", znany.GetTaxOfficeName());
                Assert.Contains("urząd skarbowy", nieznany.GetTaxOfficeName(),
                    StringComparison.OrdinalIgnoreCase);
            }

            [Fact]
            public void GetTaxOfficeName_NieznanypPrefiks_ZawieraTenPrefiks()
            {
                var nip = new NIP("9990000008");
                Assert.Contains(nip.GetTaxOfficeNumber(), nip.GetTaxOfficeName());
            }

            [Fact]
            public void GetTaxOfficeName_NieZwracaNull()
            {
                var nip = new NIP("1010000002");
                Assert.NotNull(nip.GetTaxOfficeName());
            }
        }

        public class Konstruktor_WyjatkiTests
        {
            [Theory]
            [InlineData(null)]
            [InlineData("")]
            [InlineData("   ")]
            public void Konstruktor_PustyLubNull_RzucaArgumentException(string? nip)
                => Assert.Throws<ArgumentException>(() => new NIP(nip!));

            [Theory]
            [InlineData("101000000")]
            [InlineData("10100000020")]
            [InlineData("1010000001")]
            [InlineData("ABCDEFGHIJ")]
            [InlineData("6010000000")]
            public void Konstruktor_NiepoprawnyNIP_RzucaArgumentException(string nip)
                => Assert.Throws<ArgumentException>(() => new NIP(nip));

            [Fact]
            public void Konstruktor_PoprawnyNIP_NieRzucaWyjatku()
            {
                var ex = Record.Exception(() => new NIP("1010000002"));
                Assert.Null(ex);
            }

            [Fact]
            public void Konstruktor_WyjatekZawieraNazweParametru()
            {

                var ex = Assert.Throws<ArgumentException>(() => new NIP("1010000001"));
                Assert.Equal("value", ex.ParamName);
            }
        }

        public class Rownosc_Tests
        {
            [Fact]
            public void DwaNIPyZTymSamymNumerem_SaRowne()
            {
                var a = new NIP("1010000002");
                var b = new NIP("1010000002");
                Assert.Equal(a, b);
            }

            [Fact]
            public void DwaNIPyZRoznymNumerem_SaRozne()
            {
                var a = new NIP("1010000002");
                var b = new NIP("1020000009");
                Assert.NotEqual(a, b);
            }

            [Fact]
            public void NIPFormatowanyISanitowany_RownyRawnemu()
            {
                var sFormatem  = new NIP("101-000-00-02");
                var bezFormatu = new NIP("1010000002");
                Assert.Equal(sFormatem, bezFormatu);
            }

            [Fact]
            public void OperatorRownosci_PoprawniePorownuje()
            {
                var a = new NIP("1010000002");
                var b = new NIP("1010000002");
                Assert.True(a == b);
                Assert.False(a != b);
            }

            [Fact]
            public void OperatorNierownosci_PoprawniePorownuje()
            {
                var a = new NIP("1010000002");
                var b = new NIP("2010000008");
                Assert.True(a != b);
                Assert.False(a == b);
            }

            [Fact]
            public void GetHashCode_TeSameNIPy_TenSamHash()
            {
                var a = new NIP("1010000002");
                var b = new NIP("1010000002");
                Assert.Equal(a.GetHashCode(), b.GetHashCode());
            }
        }

        public class Formatowanie_Tests
        {
            [Fact]
            public void ToFormattedString_ZwracaFormatXXX_XXX_XX_XX()
            {
                var nip = new NIP("1010000002");
                Assert.Equal("101-000-00-02", nip.ToFormattedString());
            }

            [Fact]
            public void ToFormattedString_MaDlugosc13()
            {
                var nip = new NIP("1010000002");
                Assert.Equal(13, nip.ToFormattedString().Length);
            }

            [Fact]
            public void ToString_ZwracaCystyNumer()
            {
                var nip = new NIP("101-000-00-02");
                Assert.Equal("1010000002", nip.ToString());
            }

            [Fact]
            public void Value_PrzechowujeNumerBezFormatowania()
            {
                var nip = new NIP("101-000-00-02");
                Assert.Equal("1010000002", nip.Value);
                Assert.DoesNotContain("-", nip.Value);
            }

            [Fact]
            public void Value_MaDlugosc10()
            {
                var nip = new NIP("1010000002");
                Assert.Equal(10, nip.Value.Length);
            }
        }
    }
}