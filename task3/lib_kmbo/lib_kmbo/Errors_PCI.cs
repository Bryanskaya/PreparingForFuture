using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Errs
{
    public class ErrsPCI : BErr
    {                        
        static public void Init()
        {
            if (errs != null)
                errs.Clear();

            errs = new Dictionary<UInt64, String[]>()
            {
                 [0] = new String[2]{ "OK",
                                      "ОК"},

                 [0xE0000001L] = new String[2]{ "The zero-length buffer was passed to the driver for receiving/transmitting",
                                                "Для приема/передачи драйверу был передан буфер нулевой длины" },

                 [0xE0000002L] = new String[2]{ "The buffer with too large length was passed to the driver for receiving/transmitting",
                                                "Для приема/передачи драйверу был передан буфер слишком большой длины"},

                 [0xE0000003L] = new String[2]{ "The buffer with incorrect length was passed to the driver for executing command",
                                                "Для выполнения команды драйверу был передан буфер неверной длины"},

                 [0xE0000004L] = new String[2]{ "The incorrect data was passed to the driver for executing command",
                                                "Для выполнения команды драйверу были переданы неверные данные"},

                 [0xE0000005L] = new String[2]{ "The attribute 'Counter work' was set after the exchange",
                                                "После обмена установлен признак 'Встречная работа'"},

                 [0xE0000006L] = new String[2]{ "The attribute 'Parity bit control' was set after the exchange",
                                                "После обмена установлен признак 'Контроль бита четности'"},

                 [0xE0000007L] = new String[2]{ "No interruption at the end of the exchange",
                                                "Отсутствие прерывания по окончанию обмена"}
            };
        }
    }
    
}
