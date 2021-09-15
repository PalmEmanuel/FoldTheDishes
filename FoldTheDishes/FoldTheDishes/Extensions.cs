using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace FoldTheDishes
{
    public static class Extensions
    {
        // https://stackoverflow.com/questions/19112922/sort-observablecollectionstring-through-c-sharp/36642852#36642852
        public static void Sort<T>(this ObservableCollection<T> collection, Comparison<T> comparison)
        {
            var sortableList = new List<T>(collection);
            sortableList.Sort(comparison);

            for (int i = 0; i < sortableList.Count; i++)
            {
                collection.Move(collection.IndexOf(sortableList[i]), i);
            }
        }
        public static DateTime TrimToMinutes(this DateTime datetime)
        {
            return new DateTime(datetime.Year, datetime.Month, datetime.Day, datetime.Hour, datetime.Minute, 0);
        }
    }
}
