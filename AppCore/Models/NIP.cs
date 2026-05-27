using System;
using System.Collections.Generic;
using System.Linq;

namespace AppCore.Models
{
    public sealed class NIP : IEquatable<NIP>
    {
        public string Value { get; }

        // ── Konstruktor ──────────────────────────────────────────────────────────
        public NIP(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("NIP nie może być pusty.", nameof(value));

            string sanitized = Sanitize(value);

            if (!IsValid(sanitized))
                throw new ArgumentException($"Nieprawidłowy NIP: '{sanitized}'", nameof(value));

            Value = sanitized;
        }

        // ── Statyczne metody pomocnicze ──────────────────────────────────────────

        /// <summary>Usuwa myślniki, spacje i inne znaki niebędące cyframi.</summary>
        public static string Sanitize(string nip)
        {
            if (nip is null) return string.Empty;
            // Zostawiamy wyłącznie cyfry
            return new string(nip.Where(char.IsDigit).ToArray());
        }

        /// <summary>Sprawdza poprawność NIP-u (format + cyfra kontrolna).</summary>
        public static bool IsValid(string nip)
        {
            nip = Sanitize(nip);

            if (nip.Length != 10)
                return false;

            int[] weights = { 6, 5, 7, 2, 3, 4, 5, 6, 7 };

            int controlSum = 0;
            for (int i = 0; i < weights.Length; i++)
                controlSum += weights[i] * (nip[i] - '0');

            int controlDigit = controlSum % 11;

            // Wynik 10 oznacza niepoprawny NIP
            if (controlDigit == 10)
                return false;

            return controlDigit == (nip[9] - '0');
        }

        // ── Metody domenowe ──────────────────────────────────────────────────────

        /// <summary>Zwraca trzycyfrowy prefiks identyfikujący urząd skarbowy.</summary>
        public string GetTaxOfficeNumber() => Value.Substring(0, 3);

        /// <summary>Zwraca pełną nazwę urzędu skarbowego na podstawie prefiksu.</summary>
        public string GetTaxOfficeName()
        {
            return TaxOfficePrefixes.TryGetValue(GetTaxOfficeNumber(), out var name)
                ? name
                : $"Nieznany urząd skarbowy (prefiks: {GetTaxOfficeNumber()})";
        }

        // ── Formatowanie ─────────────────────────────────────────────────────────

        /// <summary>Zwraca NIP w formacie XXX-XXX-XX-XX.</summary>
        public string ToFormattedString()
            => $"{Value[..3]}-{Value[3..6]}-{Value[6..8]}-{Value[8..10]}";

        public override string ToString() => Value;

        // ── Równość (ValueObject) ────────────────────────────────────────────────

        public bool Equals(NIP? other) => other is not null && Value == other.Value;

        public override bool Equals(object? obj) => obj is NIP other && Equals(other);

        public override int GetHashCode() => Value.GetHashCode();

        public static bool operator ==(NIP? left, NIP? right)
            => left is null ? right is null : left.Equals(right);

        public static bool operator !=(NIP? left, NIP? right) => !(left == right);

        // ── Słownik urzędów skarbowych ───────────────────────────────────────────

        private static readonly Dictionary<string, string> TaxOfficePrefixes = new()
        {
           
            { "101", "Pierwszy Urząd Skarbowy w Warszawie" },
            { "102", "Drugi Urząd Skarbowy w Warszawie" },
            { "103", "Urząd Skarbowy Warszawa-Mokotów" },
            { "104", "Urząd Skarbowy Warszawa-Praga" },
            { "105", "Urząd Skarbowy Warszawa-Śródmieście" },
            { "106", "Urząd Skarbowy Warszawa-Ursynów" },
            { "107", "Urząd Skarbowy Warszawa-Wola" },
            { "108", "Urząd Skarbowy Warszawa-Żoliborz" },
            { "109", "Urząd Skarbowy Warszawa-Bemowo" },
            { "110", "Urząd Skarbowy Warszawa-Bielany" },
            { "111", "Urząd Skarbowy Warszawa-Targówek" },
            { "112", "Trzeci Urząd Skarbowy Warszawa-Śródmieście" },
            { "121", "Urząd Skarbowy w Grodzisku Mazowieckim" },
            { "122", "Urząd Skarbowy w Legionowie" },
            { "123", "Urząd Skarbowy w Ostrołęce" },
            { "124", "Urząd Skarbowy w Płocku" },
            { "125", "Urząd Skarbowy w Radomiu" },
            { "126", "Urząd Skarbowy w Siedlcach" },
            { "201", "Pierwszy Urząd Skarbowy w Krakowie" },
            { "202", "Drugi Urząd Skarbowy w Krakowie" },
            { "203", "Trzeci Urząd Skarbowy w Krakowie" },
            { "204", "Czwarty Urząd Skarbowy w Krakowie" },
            { "205", "Piąty Urząd Skarbowy w Krakowie" },
            { "206", "Urząd Skarbowy Kraków-Krowodrza" },
            { "207", "Urząd Skarbowy Kraków-Nowa Huta" },
            { "208", "Urząd Skarbowy Kraków-Podgórze" },
           
        };
    }
}