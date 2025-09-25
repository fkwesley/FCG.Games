using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Helpers
{
    public static class DateTimeHelper
    {
        /// <summary>
        /// Converte uma data/hora UTC para o fuso horário informado.
        /// </summary>
        /// <param name="utcDateTime">Data/hora em UTC</param>
        /// <param name="timeZoneId">ID do timezone (ex: "E. South America Standard Time" para São Paulo)</param>
        /// <returns>DateTime convertido para o timezone especificado</returns>
        public static DateTime ConvertUtcToTimeZone(DateTime utcDateTime, string timeZoneId)
        {
            if (utcDateTime.Kind != DateTimeKind.Utc)
                throw new ArgumentException("A data deve ser do tipo UTC.", nameof(utcDateTime));

            var timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
            var converted = TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, timeZoneInfo);

            // Especificar que o Kind é Local (ou seja, "horário local do sistema")
            return DateTime.SpecifyKind(converted, DateTimeKind.Local);
        }

        /// <summary>
        /// Converte uma data/hora local para UTC com base no timezone informado.
        /// </summary>
        /// <param name="localDateTime">Data/hora local</param>
        /// <param name="timeZoneId">ID do timezone (ex: "E. South America Standard Time")</param>
        /// <returns>DateTime em UTC</returns>
        public static DateTime ConvertTimeZoneToUtc(DateTime localDateTime, string timeZoneId)
        {
            var timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
            return TimeZoneInfo.ConvertTimeToUtc(localDateTime, timeZoneInfo);
        }
    }
}
