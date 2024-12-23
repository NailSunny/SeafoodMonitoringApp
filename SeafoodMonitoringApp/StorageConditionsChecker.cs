using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeafoodMonitoringApp
{
    public class StorageConditionsChecker
    {
        public string CheckStorageConditions(string fishType, int maxTemp, int maxTime, int minTemp, int minTime,
        DateTime startDateTime, List<int> temperatures)
        {
            var report = new StringBuilder();
            report.AppendLine($"Отчет по условиям хранения для: {fishType}");
            report.AppendLine($"Дата/время начала: {startDateTime}");

            int violationMaxTime = 0; // Время превышения максимальной температуры
            int violationMinTime = 0; // Время превышения минимальной температуры
            int consecutiveAboveMax = 0; // Непрерывное время превышения максимальной температуры
            int consecutiveBelowMin = 0; // Непрерывное время ниже минимальной температуры

            for (int i = 0; i < temperatures.Count; i++)
            {
                var temp = temperatures[i];
                var time = startDateTime.AddMinutes(i * 10);

                if (temp > maxTemp)
                {
                    consecutiveAboveMax += 10;
                    report.AppendLine($"Превышение максимальной температуры: {temp}°C (в {time})");
                }
                else
                {
                    violationMaxTime += consecutiveAboveMax;
                    consecutiveAboveMax = 0;
                }

                if (temp < minTemp)
                {
                    consecutiveBelowMin += 10;
                    report.AppendLine($"Ниже минимальной температуры: {temp}°C (в {time})");
                }
                else
                {
                    violationMinTime += consecutiveBelowMin;
                    consecutiveBelowMin = 0;
                }
            }

            // Учитываем последние накопленные превышения
            violationMaxTime += consecutiveAboveMax;
            violationMinTime += consecutiveBelowMin;

            // Формируем финальный отчет по итогам анализа
            if (violationMaxTime > maxTime)
            {
                int exceededMaxTime = violationMaxTime - maxTime;
                report.AppendLine($"Порог максимально допустимой температуры превышен на {exceededMaxTime} минут от допустимого значения.");
            }
            else
            {
                report.AppendLine("Максимальная температура соблюдена.");
            }

            if (violationMinTime > minTime)
            {
                int exceededMinTime = violationMinTime - minTime;
                report.AppendLine($"Порог минимально допустимой температуры превышен на {exceededMinTime} минут от допустимого значения.");
            }
            else
            {
                report.AppendLine("Минимальная температура соблюдена.");
            }

            return report.ToString();
        }
    }
}
