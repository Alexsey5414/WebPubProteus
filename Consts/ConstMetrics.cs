using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebTestProteus.Consts
{
    public static class ConstMetrics
    {  //загрузка шаблона отчета
        public const string load = "load";
       //рендеринг
        public const string prepare = "prepare";
        //запуск отчета
        public const string runmacros = "runmacros";
        //SQL запросы
        public const string sql = "sql";
        //экспорт отчетов в формат
        public const string export = "export";
        //количество потоков
        public const string threadcount = "threadcount";
    }

    public static class ConstParameters
    {
        //Возврат DataSeries по операциям
        public const string Operations = "Operations";
        //Последние операции  
        public const string LastOperations = "LastOperations";
        //Количество выполненных отчетов
        public const string DoneReportCount = "DoneReportCount";
        //Время выполненных отчетов
        public const string DoneReportDuration = "DoneReportDuration";
        //количество потоков
        public const string ThreadCount = "ThreadCount";
        //Возврат DataSeries по потокам   
        public const string Threads = "Threads";
        //Время выполнения текущих отчетов
        public const string CurrentReportDuration = "CurrentReportDuration";
        //Для фильтра
        public const string firstTopTagKeys = "Первые ТОП";
        //Количество мониторируемых операций
        public const int CountMonitorOperation = 4;

    }

    public enum Measures
    {
        Operation,
        LastOper,
        ReportCount,
        ReportDuration,
        ThreadCount,
        ThreadInfo,
        CurrentReport,
        EmptyOper
    }
}
