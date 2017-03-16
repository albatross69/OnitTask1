using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnitTask1
{
    class Program
    {
        static void Main(string[] args)
        {
            //1.Создаем матрицу и заполняем ее значениями
            const int M = 20, N = 20;
            int[,] matrix = new int[M, N];
            Random r = new Random();
            for (int i = 0; i < M; i++)
            {
                for (int j = 0; j < N; j++)
                {
                    matrix[i, j] = r.Next(0, 99);
                }
            }

            for (int i = 0; i < M; i++)
            {
                for (int j = 0; j < N; j++)
                {
                    Console.Write("{0}\t", matrix[i, j]);
                }
                Console.WriteLine();
            }

            //2.Ищем максимальные значения в строках и заоминаем их индексы
            //В ключе храним число, и список с его индексами в строке
            Dictionary<int, List<int>> max_in_row_indexes = new Dictionary<int, List<int>>();
            for (int i = 0; i < 5; i++)
            {
                int max = 0;
                int max_i = 0, max_j = 0;
                for (int j = 0; j < 5; j++)
                {
                    if (matrix[i, j] > max)
                    {
                        max = matrix[i, j];
                        max_i = i;
                        max_j = j;
                    }
                }

                max_in_row_indexes[max] = new List<int>();
                max_in_row_indexes[max].Add(max_i);
                max_in_row_indexes[max].Add(max_j);
            }

            

            //3.Найти максимальный элемент, соседний с максимумом текущей строки
            //3.1. Обойти все соседние элементы, и поместить их в список
            //Данный словарь хранит в ключе произведение 4-к чисел
            //А в значении - список множителей
            
            Dictionary<long, List<int>> multiplies_and_factors = new Dictionary<long, List<int>>();
            List<int> factors = new List<int>();//список множителей
            long multiple;//произведение
            foreach (var key in max_in_row_indexes.Keys)
            {
                int elem = key, row_index = max_in_row_indexes[key][0], col_index = max_in_row_indexes[key][1];
                multiple = key;
                //Список индексов элементов, которые уже были добавлены в список множителей
                List<MyPair> stop_list = new List<MyPair>();
                factors.Add(key);
                for (int i = 0; i < 3; i++)
                {
                    stop_list.Add(new MyPair(row_index, col_index));
                    List<int> next_neighbour = Around_Max(matrix, M, N, elem, row_index, col_index, stop_list);
                    factors.Add(next_neighbour[0]);
                    multiple *= next_neighbour[0];
                    elem = next_neighbour[0];
                    row_index = next_neighbour[1];
                    col_index = next_neighbour[2];
                    next_neighbour.Clear();
                }
                //Вспомогательный список, потребовался из-за того, что
                //при каждом новом поиске список множителей нужно очищать
                //и добавлять ссылку на него в словарь нельзя
                List<int> kostyl = new List<int>();
                if (!HasAKey(multiplies_and_factors, multiple))
                    multiplies_and_factors.Add(multiple, kostyl);
                foreach (var item in factors)
                {
                     multiplies_and_factors[multiple].Add(item);
                }
                factors.Clear();
            }

            //Выводим множители самого большого произведения
            Console.Write("Самое большое произведение: {0} = ", multiplies_and_factors.Keys.Max());
            foreach (var item in multiplies_and_factors[multiplies_and_factors.Keys.Max()])
            {
                Console.Write("{0} * ", item);
            }

            Console.ReadLine();
        }

        //Сравнение текущих индексов с индексами использованных элементов
        public static bool IsEqual(int i1, int i2, List<MyPair> saved_indexes)
        {
            foreach (var pair in saved_indexes)
            {
                if (i1 == pair.x && i2 == pair.y)
                {
                    return true;
                }
            }
            return false;
        }

        public static bool HasAKey(Dictionary<long, List<int>> dic, long value)
        {
            foreach (var key in dic.Keys)
            {
                if (value == key)
                {
                    return true;
                }
            }
            return false;
        }

        //Принимает матрицу, кол-во строк, кол-во столбцов, элемент, вокруг которого
        //будет поиск наибольшего соседа и его индексы, а также список индексов элементов,
        //которые были добавлены в список множителей
        //Возвращает список, первый элемент которого - наибольший сосед, а два других - его координаты
        public static List<int> Around_Max(int[,] matrix, int M, int N, int elem, int row_index, int col_index, List<MyPair> stop_list)
        {
            Dictionary<int, MyPair> around_elems_2 = new Dictionary<int, MyPair>();
            List<int> around_elems = new List<int>();//Список соседних элементов
            //получаем позицию текущего элемента
            int i_cur = row_index;
            int j_cur = col_index;
            int max_around, max_ar_i = 0, max_ar_j = 0;
            List<int> ret_list = new List<int>();
            //если это первая строка и первый столбец
            if (i_cur == 0 && j_cur == 0)
            {
                if (!IsEqual(i_cur, j_cur + 1, stop_list))
                {
                    around_elems_2[matrix[i_cur, j_cur + 1]] = new MyPair(i_cur, j_cur + 1);
                }
                if (!IsEqual(i_cur + 1, j_cur + 1, stop_list))
                {
                    around_elems_2[matrix[i_cur + 1, j_cur + 1]] = new MyPair(i_cur + 1, j_cur + 1);
                }
                if (!IsEqual(i_cur + 1, j_cur, stop_list))
                {
                    around_elems_2[matrix[i_cur + 1, j_cur]] = new MyPair(i_cur + 1, j_cur);
                }
                //Находим максимальный элемент и его индексы
                max_around = around_elems_2.Keys.Max();
                max_ar_i = around_elems_2[max_around].x;
                max_ar_j = around_elems_2[max_around].y;
                
                ret_list.Add(max_around);
                ret_list.Add(max_ar_i);
                ret_list.Add(max_ar_j);

                return ret_list;
            }
            //если это первая строка и последний столбец
            if (i_cur == 0 && j_cur == N - 1)
            {
                if (!IsEqual(i_cur, j_cur - 1, stop_list))
                    around_elems_2[matrix[i_cur, j_cur - 1]] = new MyPair(i_cur, j_cur - 1);
                if (!IsEqual(i_cur, j_cur - 1, stop_list))
                    around_elems_2[matrix[i_cur + 1, j_cur - 1]] = new MyPair(i_cur, j_cur - 1);
                if (!IsEqual(i_cur + 1, j_cur - 1, stop_list))
                    around_elems_2[matrix[i_cur + 1, j_cur - 1]] = new MyPair(i_cur + 1, j_cur - 1);
                max_around = around_elems_2.Keys.Max();
                max_ar_i = around_elems_2[max_around].x;
                max_ar_j = around_elems_2[max_around].y;

                ret_list.Add(max_around);
                ret_list.Add(max_ar_i);
                ret_list.Add(max_ar_j);

                return ret_list;
            }
            //если это последняя строка и первый столбец
            if (i_cur == M - 1 && j_cur == 0)
            {
                if (!IsEqual(i_cur, j_cur + 1, stop_list))
                    around_elems_2[matrix[i_cur, j_cur + 1]] = new MyPair(i_cur, j_cur + 1);
                if (!IsEqual(i_cur - 1, j_cur + 1, stop_list))
                    around_elems_2[matrix[i_cur - 1, j_cur + 1]] = new MyPair(i_cur - 1, j_cur + 1);
                if (!IsEqual(i_cur - 1, j_cur, stop_list))
                    around_elems_2[matrix[i_cur - 1, j_cur]] = new MyPair(i_cur - 1, j_cur);
                //3.2 Находим максимальный элемент и его индексы
                max_around = around_elems_2.Keys.Max();
                max_ar_i = around_elems_2[max_around].x;
                max_ar_j = around_elems_2[max_around].y;

                ret_list.Add(max_around);
                ret_list.Add(max_ar_i);
                ret_list.Add(max_ar_j);

                return ret_list;
            }
            //если это последняя строка и последний столбец
            if (i_cur == M - 1 && j_cur == N - 1)
            {
                if (!IsEqual(i_cur, j_cur - 1, stop_list))
                    around_elems_2[matrix[i_cur, j_cur - 1]] = new MyPair(i_cur, j_cur - 1);
                if (!IsEqual(i_cur - 1, j_cur - 1, stop_list))
                    around_elems_2[matrix[i_cur - 1, j_cur - 1]] = new MyPair(i_cur - 1, j_cur - 1);
                if (!IsEqual(i_cur - 1, j_cur, stop_list))
                    around_elems_2[matrix[i_cur - 1, j_cur]] = new MyPair(i_cur - 1, j_cur);
                //3.2 Находим максимальный элемент и его индексы
                max_around = around_elems_2.Keys.Max();
                max_ar_i = around_elems_2[max_around].x;
                max_ar_j = around_elems_2[max_around].y;

                ret_list.Add(max_around);
                ret_list.Add(max_ar_i);
                ret_list.Add(max_ar_j);

                return ret_list;
            }
            //если это Первая строка 
            if (i_cur == 0)
            {
                if (!IsEqual(i_cur, j_cur - 1, stop_list))
                    around_elems_2[matrix[i_cur, j_cur - 1]] = new MyPair(i_cur, j_cur - 1);
                if (!IsEqual(i_cur, j_cur + 1, stop_list))
                    around_elems_2[matrix[i_cur, j_cur + 1]] = new MyPair(i_cur, j_cur + 1);
                if (!IsEqual(i_cur + 1, j_cur - 1, stop_list))
                    around_elems_2[matrix[i_cur + 1, j_cur - 1]] = new MyPair(i_cur + 1, j_cur - 1);
                if (!IsEqual(i_cur + 1, j_cur, stop_list))
                    around_elems_2[matrix[i_cur + 1, j_cur]] = new MyPair(i_cur + 1, j_cur);
                if (!IsEqual(i_cur + 1, j_cur + 1, stop_list))
                    around_elems_2[matrix[i_cur + 1, j_cur + 1]] = new MyPair(i_cur + 1, j_cur + 1);
                //Находим максимальный элемент и его индексы
                max_around = around_elems_2.Keys.Max();
                max_ar_i = around_elems_2[max_around].x;
                max_ar_j = around_elems_2[max_around].y;

                ret_list.Add(max_around);
                ret_list.Add(max_ar_i);
                ret_list.Add(max_ar_j);

                return ret_list;
            }
            //если это Последняя строка 
            if (i_cur == M - 1)
            {
                if (!IsEqual(i_cur, j_cur - 1, stop_list))
                    around_elems_2[matrix[i_cur, j_cur - 1]] = new MyPair(i_cur, j_cur - 1);
                if (!IsEqual(i_cur, j_cur + 1, stop_list))
                    around_elems_2[matrix[i_cur, j_cur + 1]] = new MyPair(i_cur, j_cur + 1);
                if (!IsEqual(i_cur - 1, j_cur - 1, stop_list))
                    around_elems_2[matrix[i_cur - 1, j_cur - 1]] = new MyPair(i_cur - 1, j_cur - 1);
                if (!IsEqual(i_cur - 1, j_cur, stop_list))
                    around_elems_2[matrix[i_cur - 1, j_cur]] = new MyPair(i_cur - 1, j_cur);
                if (!IsEqual(i_cur - 1, j_cur + 1, stop_list))
                    around_elems_2[matrix[i_cur - 1, j_cur + 1]] = new MyPair(i_cur - 1, j_cur + 1);
                //Находим максимальный элемент и его индексы
                max_around = around_elems_2.Keys.Max();
                max_ar_i = around_elems_2[max_around].x;
                max_ar_j = around_elems_2[max_around].y;

                ret_list.Add(max_around);
                ret_list.Add(max_ar_i);
                ret_list.Add(max_ar_j);

                return ret_list;
            }
            //если это Первый столбец 
            if (j_cur == 0)
            {
                if (!IsEqual(i_cur + 1, j_cur, stop_list))
                    around_elems_2[matrix[i_cur + 1, j_cur]] = new MyPair(i_cur + 1, j_cur);
                if (!IsEqual(i_cur - 1, j_cur, stop_list))
                    around_elems_2[matrix[i_cur - 1, j_cur]] = new MyPair(i_cur - 1, j_cur);
                if (!IsEqual(i_cur + 1, j_cur + 1, stop_list))
                    around_elems_2[matrix[i_cur + 1, j_cur + 1]] = new MyPair(i_cur + 1, j_cur + 1);
                if (!IsEqual(i_cur, j_cur + 1, stop_list))
                    around_elems_2[matrix[i_cur, j_cur + 1]] = new MyPair(i_cur, j_cur + 1);
                if (!IsEqual(i_cur - 1, j_cur + 1, stop_list))
                    around_elems_2[matrix[i_cur - 1, j_cur + 1]] = new MyPair(i_cur - 1, j_cur + 1);
                //Находим максимальный элемент и его индексы
                max_around = around_elems_2.Keys.Max();
                max_ar_i = around_elems_2[max_around].x;
                max_ar_j = around_elems_2[max_around].y;

                ret_list.Add(max_around);
                ret_list.Add(max_ar_i);
                ret_list.Add(max_ar_j);

                return ret_list;
            }
            //если это Последний столбец 
            if (j_cur == N - 1)
            {
                if (!IsEqual(i_cur + 1, j_cur, stop_list))
                    around_elems_2[matrix[i_cur + 1, j_cur]] = new MyPair(i_cur + 1, j_cur);
                if (!IsEqual(i_cur - 1, j_cur, stop_list))
                    around_elems_2[matrix[i_cur - 1, j_cur]] = new MyPair(i_cur - 1, j_cur);
                if (!IsEqual(i_cur + 1, j_cur - 1, stop_list))
                    around_elems_2[matrix[i_cur + 1, j_cur - 1]] = new MyPair(i_cur + 1, j_cur - 1);
                if (!IsEqual(i_cur, j_cur - 1, stop_list))
                    around_elems_2[matrix[i_cur, j_cur - 1]] = new MyPair(i_cur, j_cur - 1);
                if (!IsEqual(i_cur - 1, j_cur - 1, stop_list))
                    around_elems_2[matrix[i_cur - 1, j_cur - 1]] = new MyPair(i_cur - 1, j_cur - 1);
                //Находим максимальный элемент и его индексы
                max_around = around_elems_2.Keys.Max();
                max_ar_i = around_elems_2[max_around].x;
                max_ar_j = around_elems_2[max_around].y;

                ret_list.Add(max_around);
                ret_list.Add(max_ar_i);
                ret_list.Add(max_ar_j);

                return ret_list;
            }
            //В общем случае: 
            else
            {
                if (!IsEqual(i_cur - 1, j_cur - 1, stop_list))
                    around_elems_2[matrix[i_cur - 1, j_cur - 1]] = new MyPair(i_cur - 1, j_cur - 1);
                if (!IsEqual(i_cur - 1, j_cur, stop_list))
                    around_elems_2[matrix[i_cur - 1, j_cur]] = new MyPair(i_cur - 1, j_cur);
                if (!IsEqual(i_cur - 1, j_cur + 1, stop_list))
                    around_elems_2[matrix[i_cur - 1, j_cur + 1]] = new MyPair(i_cur - 1, j_cur + 1);
                if (!IsEqual(i_cur + 1, j_cur - 1, stop_list))
                    around_elems_2[matrix[i_cur + 1, j_cur - 1]] = new MyPair(i_cur + 1, j_cur - 1);
                if (!IsEqual(i_cur + 1, j_cur, stop_list))
                    around_elems_2[matrix[i_cur + 1, j_cur]] = new MyPair(i_cur + 1, j_cur);
                if (!IsEqual(i_cur + 1, j_cur + 1, stop_list))
                    around_elems_2[matrix[i_cur + 1, j_cur + 1]] = new MyPair(i_cur + 1, j_cur + 1);
                if (!IsEqual(i_cur, j_cur - 1, stop_list))
                    around_elems_2[matrix[i_cur, j_cur - 1]] = new MyPair(i_cur, j_cur - 1);
                if (!IsEqual(i_cur, j_cur + 1, stop_list))
                    around_elems_2[matrix[i_cur, j_cur + 1]] = new MyPair(i_cur, j_cur + 1);
                //3.2 Находим максимальный элемент и его индексы
                max_around = around_elems_2.Keys.Max();
                max_ar_i = around_elems_2[max_around].x;
                max_ar_j = around_elems_2[max_around].y;

                ret_list.Add(max_around);
                ret_list.Add(max_ar_i);
                ret_list.Add(max_ar_j);

                return ret_list;
            }
        }
    }
}
