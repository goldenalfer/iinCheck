# iinCheck
Функция проверки и извлечения данных из ИИН на C#. ИИН - это Индивидуальный Идентификационный Номер граждан Республики Казахстан (РК) и юридических лиц РК

## Использование функции
```
var iinHelper = new IINHelper();
var result = iinHelper.Check("iin");
//result.IsValid - корректен ли ИИН
//result.IsPerson - true - физ лицо, false - Юр лицо
//result.BirthDate - дата рождения физ лица
//result.IINSex - пол физ лица (enum: 0 - не определено, 1 - мужчина, 2 - женщина)
//result.ErrorMessage - описание ошибки, если ИИН некорректен
```
## Алгоритм формирования ИИН
Алгоритм формирования ИИН регулируется Постановлением правительства РК "Об утверждении Программы перехода на единый номер физического (юридического) лица (индивидуальный идентификационный номер (бизнес-идентификационный номер) в целях создания Национальных реестров идентификационных номеров Республики Казахстан" от 11 июня 2003 года N 565
[https://adilet.zan.kz/rus/docs/P030000565_](https://adilet.zan.kz/rus/docs/P030000565_)

![img](https://github.com/goldenalfer/iinCheck/blob/main/inn_schema.png)

### Физ лица и Индивидуальные Предприниматели (ИИН)
1) первый фасет - содержит 6 разрядов (с а1 по а6) и характеризует год (две последние цифры), месяц и дату рождения физического лица;
2) второй фасет - содержит 1 (а7) разряд и характеризует пол физического лица и век его рождения. Причем при определении значения разряда для мужчин используются нечетные цифры, а для женщин - четные цифры.
Конкретные значения данного разряда выглядят следующим образом:
1 - для мужчин, родившихся в 19 веке;
2 - для женщин, родившихся в 19 веке;
3 - для мужчин, родившихся в 20 веке;
4 - для женщин, родившихся в 20 веке;
5 - для мужчин, родившихся в 21 веке;
6 - для женщин, родившихся в 21 веке.
Следовательно, в данном разряде имеются резервные значения 7, 8, 9 и 0;
3) третий фасет - содержит 4 разряда (с а8 по а11) и характеризует порядковый номер регистрации в системе. По данным Агентства по статистике Республики Казахстан максимальное количество родившихся в один день (начиная с 1999 года) - 1229 человек, поэтому 4 знака для порядкового номера регистрации вполне достаточно. Порядковый номер регистрации проставляется сплошной нумерацией в рамках одной группы (года рождения);
4) четвертый фасет - содержит 1 разряд (а12) и является контрольным разрядом ИИН. Алгоритм расчета контрольного разряда ИИН приведен в параграфе 5 главы 5 Программы.

Индивидуальные предприниматели, осуществляющие свою деятельность в форме личного предпринимательства (далее - ИП(Л)), будут использовать ИИН, который был присвоен физическому лицу при рождении.

### Юридические лица (БИН)
Предлагается следующий алгоритм генерирования БИН:
1) первый фасет - содержит 4 разряда (с а1 по а4) и характеризует год (две последние цифры) и месяц регистрации юридического лица или индивидуального предпринимателя, осуществляющего деятельность на основе совместного предпринимательства (далее - ЮЛ или ИП(С));
2) второй фасет - содержит 1 разряд (а5) и характеризует тип ЮЛ или ИП(С). Кроме того, алгоритм задания значения данного разряда также используется для исключения возможности совпадения БИН с ИИН.
Поскольку в структуре ИИН а5 (пятый разряд) означает первую цифру даты рождения, то, учитывая, что дата рождения может начинаться только с 0, 1, 2 или 3 (например, 01, 11, 21, 31), обязательным условием в генерировании в структуре БИН а5 (пятого разряда) будет исключение использования цифр 0, 1, 2 и 3.
Предлагается использовать следующие конкретные значения данного разряда:
4 - для юридических лиц-резидентов;
5 - для юридических лиц-нерезидентов;
6 - для индивидуальных предпринимателей, осуществляющих деятельность на основе совместного предпринимательства.
Следовательно, в данном разряде имеются резервные значения 7, 8 и 9;
3) третий фасет - содержит 1 разряд (а6) и характеризует специальный признак, являющийся дополнительной детализацией разряда а5:
0 - признак головного подразделения ЮЛ или ИП(С);
1 - признак филиала ЮЛ или ИП(С);
2 - признак представительства ЮЛ или ИП(С);
3 - признак иного обособленного структурного подразделения ЮЛ или ИП(С);
4 - признак крестьянского (фермерского) хозяйства, осуществляющего деятельность на основе совместного предпринимательства;
4) четвертый фасет - содержит 5 разрядов (с а7 по а11) и характеризует порядковый номер регистрации в системе ЮЛ или ИП(С), а также их структурных подразделений;
5) пятый фасет - содержит 1 разряд (а12) и является контрольным разрядом БИН. Алгоритм расчета контрольного разряда БИН приведен в параграфе 5 главы 5 Программы и аналогичен расчету контрольного разряда ИИН.

### Алгоритм расчета значения контрольного разряда
В целях осуществления контроля и снижения ошибок клавиатурного ввода в составе ИИН (БИН) предусматривается наличие контрольного 12-го разряда, при расчете которого будет использоваться следующий алгоритм в два цикла:
а12=(а1*b1+а2*b2+а3*b3+а4*b4+а5*b5+а6*b6+а7*b7+а8*b8+а9*b9+a10*b10+a11*b11) mod 11,
где: ai - значение i-гo разряда;
bi - вес i-гo разряда.
разряд ИИН: 1 2 3 4 5 6 7 8 9 10 11
вес разряда: 1 2 3 4 5 6 7 8 9 10 11.
1. Если полученное число равно 10, то расчет контрольного разряда производится с другой последовательностью весов:
разряд ИИН: 1 2 3 4 5 6 7  8   9   10 11
вес разряда: 3 4 5 6 7 8 9 10 11  1    2.
2. Если полученное число также равно 10, то данный ИИН не используется.
3. Если полученное число имеет значение от 0 до 9, то данное число берется в качестве контрольного разряда.
