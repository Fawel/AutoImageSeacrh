# AutoImageSeacrh
Ищем картинки (в частности с диска) большего разрешения, чем есть. 

# Как работает?

* Получаем список картинок для поиска в виде списка папок и ссылок на картинки в сети.
* Пытаемся найти пикчу в сети с большим разрешением
* Если нашли - логируем в списке рез-тов им файла, текущее разрешение, новое разрешение, ссылка на файл с большим разрешением
* Опционально, если позволяет источник и настройки - скачка картинки и доп настройки действия с файлом 
* Перемещаем файл в папку с обработанными пикчами

# Возможные настройки

* Путь файла лога результата работы программы
* Опции что делать с обработанными и найденными файлами: не трогать, переместить в отдельную папку (нужно указать куда), удалить
* Настройки источников картинок: 
  * Сразу качать искомую пикчу, если возможно
  * Опции что делать с обработанными файлами этого источника, если пикча скачана
  * Опции что делать с обработанными файлами этого источника, если пикча найдена и скачка запрещена
  
