# Virtual Physical Lab

Unity 2022.3.20f1 asosidagi virtual fizika laboratoriyalari loyihasi.

## Holati

Loyiha hozircha ishlab chiqish bosqichidagi Unity prototipi. Repozitoriyda 21 ta laboratoriya yo'nalishi uchun kodlar va umumiy hisobot modullari mavjud. To'liq tayyor mahsulot deb hisoblashdan oldin Unity Editor'da sahnalar, Inspector bog'lanishlari, paketlar va platforma build'lari tekshirilishi shart.

## Texnik tuzatishlar

- Assets/_Core va Assets/Assets/_Core ichidagi takroriy Core sinflari olib tashlandi; bu bir xil namespace/type nomlari sababli C# kolliziyalarini bartaraf etadi.
- Fizik hisob-kitoblarda null, nol, manfiy, NaN va Infinity holatlari tekshiriladi.
- Galiley tajribasida qayta ishga tushirish oldingi coroutine'ni to'xtatadi.
- HTML hisobotida foydalanuvchi matni HTML-escape qilinadi; eksport hozircha PDF emas, chop etishga mos HTML.
- CSV maydonlari escape qilinadi va spreadsheet formula injection xavfi kamaytiriladi.
- Hisobotlarni saqlash va o'qishdagi xatolar yaxshiroq boshqariladi.
- CI WebGL va Android build'larini PR, main push va qo'lda ishga tushirish uchun sozlangan.

## Ishga tushirish

1. Unity Hub orqali Unity 2022.3.20f1 o'rnating.
2. Repozitoriyni Unity Hub'dan oching.
3. Console oynasidagi compile xatolarini tuzating.
4. Har bir laboratoriya uchun .unity sahna yarating va kerakli GameObject/Inspector referenslarini ulang.
5. File > Build Settings orqali sahnalarni qo'shing.
6. WebGL yoki Android platformasini tanlab build qiling.

## CI talablari

GitHub Actions uchun repository Secrets ichida UNITY_LICENSE kerak. Android build uchun Unity o'rnatmasida Android Build Support, SDK, NDK va OpenJDK bo'lishi kerak.

## Hozirgi cheklovlar

- Repozitoriyda .unity sahnalari va Unity .meta GUID fayllari yetarli emas; kodlar kompilyatsiya qilinsa ham tayyor interaktiv laboratoriya UI avtomatik hosil bo'lmaydi.
- PDFReportGenerator hozircha chop etishga mos HTML yaratadi, PDF binary fayl emas.
- Unity Editor build/test muhiti bu o'zgarishlar davomida ishga tushirilmagan. GitHub Actions natijasi bilan tasdiqlanmaguncha build muvaffaqiyatli deb hisoblanmaydi.
