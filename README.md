# Virtual Physical Lab

Unity 2022.3.20f1 asosidagi virtual fizika laboratoriyalari prototipi.

## Hozirgi holati

Loyihada 21 ta laboratoriya uchun fizik hisoblash skriptlari, umumiy o'lchov va hisobot modullari mavjud. Unity Editor build jarayonida avtomatik ravishda bosh menyu va 21 ta laboratoriya sahnasini yaratadi.

## Unity loyihasini ochish

1. Unity Hub orqali Unity 2022.3.20f1 versiyasini o'rnating.
2. Repozitoriy ildizidagi papkani Unity Hub orqali oching.
3. Unity birinchi marta ochilganda Package Manager Packages/manifest.json dagi TextMeshPro va UGUI paketlarini yuklab olishini kuting.
4. Tools → Virtual Physical Lab → Generate Lab Scenes menyusidan sahnalarni yaratish (yoki build pipeline orqali avtomatik yaratish) mumkin.
5. Assets/Scenes/MainMenu.unity sahnasini ochib Play tugmasini bosing.

## Build

- WebGL: Unity Build Settings'da WebGL platformasini tanlang.
- Android: Android Build Support, SDK, NDK va OpenJDK o'rnatilgan bo'lishi kerak.
- GitHub Actions uchun repository secrets: UNITY_LICENSE, UNITY_EMAIL va UNITY_PASSWORD talab qilinadi. Ularni hech qachon kodga yozmang yoki chatga yubormang.
- CI WebGL va Android uchun alohida build ishlarini bajaradi va natijalarni artifact sifatida saqlaydi.

## Tuzatishlar

- Core fizik hisoblashlarda noto'g'ri, cheksiz va nolga teng qiymatlar tekshiriladi.
- Galiley simulyatsiyasida qayta boshlashdan oldingi coroutine to'xtatiladi.
- Hisobot HTML'ida foydalanuvchi kiritgan matn HTML-escape qilinadi. Eslatma: bu PDF emas, chop etishga mos HTML.
- CSV eksportida maydonlarni escape qilish va spreadsheet formula injection himoyasi mavjud.
- Hisobotlarni saqlash/o'qishdagi xatolar boshqariladi.
- DOTween tashqi bog'liqligi olib tashlandi; UI animatsiyalari Unity coroutine orqali bajariladi.
- TextMeshPro va UGUI paketlari manifestda aniq ko'rsatilgan.
- Unity build'dan oldin bosh menyu va 21 ta boshlang'ich sahna avtomatik generatsiya qilinadi.

## Muhim cheklovlar

Avtomatik yaratiladigan laboratoriya sahnalari hozircha navigatsiya va interfeys karkasidir. Har bir tajriba uchun 3D qurilmalar, Inspector bog'lanishlari, real o'lchov boshqaruvlari va fizik skriptlar bilan to'liq integratsiya hali bajarilishi kerak. Ayrim fizik formulalar alohida skriptlarda mavjud, ammo ular barcha sahnalarga avtomatik ulanmagan.

PDFReportGenerator nomiga qaramay haqiqiy PDF fayl yaratmaydi; hozircha HTML hisobot yaratadi. Unity Editor build muvaffaqiyatli o'tmaguncha tayyor APK/WebGL mahsuloti deb hisoblamang.

## GitHub CI

GitHub Actions loglari UNITY_LICENSE mavjudligini, ammo UNITY_EMAIL va UNITY_PASSWORD secretlari yo'qligini ko'rsatdi. Shu sababli build Unity ishga tushishidan oldin to'xtaydi. Repository Settings → Secrets and variables → Actions bo'limida quyidagilarni yarating: UNITY_LICENSE (Unity .ulf faylining to'liq matni), UNITY_EMAIL (Unity hisobingiz emaili), UNITY_PASSWORD (Unity hisobingiz paroli). Bu ma'lumotlarni repozitoriy fayllariga yozmang va hech kimga yubormang. GameCI yo'riqnomasi: https://game.ci/docs/github/activation/.