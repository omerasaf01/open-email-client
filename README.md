# OpenEmail

A minimal backend-first email client project. Backend (ASP.NET) is implemented; frontend will be built with Next.js.

## Languages / Diller
- English and Türkçe sections are both included below.

---

## English

### Overview
This repository contains the backend of the OpenEmail project (see `OpenEmail-Backend/`). The frontend will be a Next.js application located in `OpenEmail-Frontend/`.

![Preview](/doc/preview.png)

### Quick start (backend)
- Ensure .NET SDK is installed.
- From `OpenEmail-Backend/OpenEmail.Api` run:

```bash
dotnet restore
dotnet build
dotnet run --project OpenEmail.Api
```

### Quick start (frontend - planned)
Create a Next.js app in `OpenEmail-Frontend/` (example using TypeScript):

```bash
cd OpenEmail-Frontend
npx create-next-app@latest . --ts
```

Recommended: use Next.js built-in i18n or `next-intl`/`next-i18next` for language support (English/Turkish).

Example recommended scripts (in `package.json` of frontend):

```json
"scripts": {
  "dev": "next dev",
  "build": "next build",
  "start": "next start"
}
```

### i18n suggestion
- For route-level translations, use Next.js i18n routing. For content translation consider `next-intl`.
- Add `locales/en` and `locales/tr` for English/Turkish JSON resources.

### Contributing
See `CONTRIBUTING.md` for guidelines on PRs, issues, and translations.

---

## Türkçe

### Genel Bakış
Bu depo backend (ASP.NET) kısmını içerir; frontend Next.js ile `OpenEmail-Frontend/` dizininde geliştirilecektir.

### Hızlı başlangıç (backend)
- .NET SDK kurulu olmalı.
- `OpenEmail-Backend/OpenEmail.Api` dizininden çalıştırın:

```bash
dotnet restore
dotnet build
dotnet run --project OpenEmail.Api
```

### Hızlı başlangıç (frontend - öneri)
Next.js uygulamasını `OpenEmail-Frontend/` içine oluşturun:

```bash
cd OpenEmail-Frontend
npx create-next-app@latest . --ts
```

### Dil desteği önerisi
- Next.js i18n veya `next-intl`/`next-i18next` kullanın. `locales/en` ve `locales/tr` dizinleri oluşturun.

---

If you want, I can scaffold the Next.js app and add a basic i18n setup.
