using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Errs
{
    public class ErrsUSB : BErr
    {
        static public void Init()
        {
            if (errs != null)
                errs.Clear();

            errs = new Dictionary<UInt64, String[]>()
            {
                [0] = new String[2]{ "OK",
                                     "ОК"},

                [1] = new String[2]{ "The descriptor is incorrect",
                                     "Неверный дескриптор" },

                [2] = new String[2]{ "The USB device is not found",
                                     "Устройство не найдено" },

                [3] = new String[2]{ "The channel is not initialized",
                                     "Канал не инициализирован" },

                [4] = new String[2]{ "Input/Output error",
                                     "Ошибка ввода-вывода" },

                [5] = new String[2]{ "Nothing to transmit",
                                     "Передать нечего" },/**/

                [6] = new String[2]{ "The resource is unavailable",
                                     "Недоступный ресурс" },/**/

                [7] = new String[2]{ "Incorrect exchange rate",
                                     "Неверная скорость обмена" },

                [8] = new String[2]{ "Erasing is not available",
                                     "Стирание недоступно" },

                [9] = new String[2]{ "Recording is impossible",
                                     "Запись невозможна" },

                [10] = new String[2]{ "Recording error",
                                      "Ошибка записи" },

                [11] = new String[2]{ "EEPROM reading is impossible",
                                      "Чтение ЭСППЗУ невозможно" },

                [12] = new String[2]{ "EEPROM recording is impossible",
                                      "Запись ЭСППЗУ невозможна" },

                [13] = new String[2]{ "EEPROM erasing is impossible",
                                      "Стирание ЭСППЗУ невозможно" },

                [14] = new String[2]{ "There is no EEPROM",
                                      "ЭСППЗУ отсутствует" },

                [15] = new String[2]{ "The EEPROM is not programmable",
                                      "ЭСППЗУ не программируется" },

                [16] = new String[2]{ "The arguments are incorrect",
                                      "Неверные аргументы" },

                [17] = new String[2]{ "Not defined mistake",
                                      "Неизвестная ошибка" },

                [18] = new String[2]{ "W/R error",
                                       "Ошибка W/R" },

                [19] = new String[2]{ "Status word is not accepted",
                                      "Слово состояния не принято" },

                [20] = new String[2]{ "Status word error",
                                      "Ошибка cлова состояния" },

                [21] = new String[2]{ "Counter work",
                                      "Встречная работа" },

                [22] = new String[2]{ "Parity bit error",
                                      "Ошибка бита четности" }
            };
        }
    }
}
