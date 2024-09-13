# PiecewiseLinearFunction project
- Код-ориентированное приложение на базе WPF.
- Code-oriented application based on WPF.
# PLF_WPForiented project
- MVVM-ориентированное приложение на базе WPF.
- MVVM-oriented application based on WPF.

Реализовано:
- создание кусочно-линейной функции путём задания ей вершин в таблице,
- проверка вводимых в редактируемую ячейку данных на соответствие Double-типу,
- добавление и удаление вершин,
- скроллинг таблицы в случае заполнения видимой области,
- добавление и удаление именных функций,
- создание инвертированной функции к текущей активной,
- выделение и копирование выделенных вершин в буфер обмена для дальнейшей вставки в другие редакторы (проверено на MS Excel и Notepad++),
- вставка скопированной информации из других текстовых редакторов (проверено на MS Excel и Notepad++) при совпадении формата,
- отображение выбранной функции на графике с учётом изменений в реальном времени,
- отображение всего списка функций на графике с учётом изменений в реальном времени,
- возможность изменения координат вершины текущей активной функции по щелчку на координатной сетке графика с занесением изменения в таблицу  (перемещения осуществляет ближайшая к указателю вершина),
- сохранение данных в файл XML формата с расширением .spf,
- загрузка данных из файла XML формата с расширением .spf,
- сброс данных,
- протокол контроля несохранённых данных.

Implemented:
- creation of a Piecewise Linear function by assigning it vertexes in the table,
- checking the data entered into the edited cell for compliance with the Double type,
- adding and deleting vertexes,
- scrolling the table if the visible area is filled,
- adding and deleting named functions,
- creating an inverted function to the current active one,
- selecting and copying selected vertexes to the clipboard for pasting this data into other editors (tested on MS Excel and Notepad++),
- pasting copied information from other text editors (tested on MS Excel and Notepad++) if the data format matches,
- displaying the selected function on the plot in realtime mode in real time mode,
- displaying the entire list of functions on the plot in realtime mode,
- the possibility to change the coordinates of the vertex of a current active function by clicking on the coordinate grid of the plot(it choosed the closest vertex to the pointer), changes are entered into the table,
- saving data to a file of XML format with .spf extension,
- loading data from file  of XML format with .spf extension,
- data reset,
- unsaved data control protocol.
