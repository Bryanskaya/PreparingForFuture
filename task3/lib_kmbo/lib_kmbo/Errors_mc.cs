using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Errs
{
    public class Errs_mc : BErr
    {
        static public void Init()
        {
            if (errs != null)
                errs.Clear();

            errs = new Dictionary<UInt64, String[]>()
            {
                [0] = new String[2]{ "OK",
                                     "ОК"},

                [0xC0000001L] = new String[2] { "Creation of socket for transfering data was FAILED",
                                                "Не удалось создать сокет для передачи данных" },

                [0xC0000002L] = new String[2] { "Binding a socket was FAILED, error in calling function 'bind'",
                                                "Не удалось связать сокет, ошибка вызова функции bind"},

                [0xC0000003L] = new String[2] { "Error in setting timeout for receiving the data from clients to socket",
                                                "Не удалось задать сокету время ожидания входящих данных от клиентов"},

                [0xC0000004L] = new String[2] { "Error in setting timeout for sending the data to clients from socket",
                                                "Не удалось задать сокету время для отправки данных клиенту"},

                [0xC0000005L] = new String[2] { "Error in creation of connection point",
                                                "Ошибка при формировании точки подключения"},

                [0xC0000006L] = new String[2] { "Connection with mc was FAILED",
                                                "Не удалось соединиться с mc"},

                [0xC0000007L] = new String[2] { "Error in initialization of module mc",
                                                "Не удалось инициализировать модуль mc"},

                [0xC0000008L] = new String[2] { "There is no available data",
                                                "Нет доступных данных"},

            };
        }
    }
}
