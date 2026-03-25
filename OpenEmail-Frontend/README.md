# OpenEmail Frontend MVP

Next.js (App Router) + shadcn/ui + React Query kullanilarak hazirlanmis email client MVP arayuzu.
Backend tarafi .NET API olacak sekilde tasarlanmistir.

## Ozellikler

- Kayit ekrani olmadan direkt login
- Korumali mail ekrani (cookie tabanli yonlendirme)
- Inbox listeleme
- Email detay goruntuleme
- Yeni email olusturma ve gonderme
- React Query ile veri cache ve mutation yonetimi

## Kurulum

1. Bagimliliklari yukleyin:

```bash
npm install
```

2. Ortam degiskenlerini hazirlayin:

```bash
cp .env.example .env.local
```

3. .NET backend endpointlerinize gore `.env.local` degerlerini guncelleyin.

4. Gelistirme sunucusunu baslatin:

```bash
npm run dev
```

## Ortam Degiskenleri

- `NEXT_PUBLIC_API_BASE_URL` (ornek: `http://localhost:5000/api`)
- `NEXT_PUBLIC_LOGIN_ENDPOINT` (varsayilan: `/auth/login`)
- `NEXT_PUBLIC_INBOX_ENDPOINT` (varsayilan: `/emails`)
- `NEXT_PUBLIC_EMAIL_DETAIL_ENDPOINT` (varsayilan: `/emails/{id}`)
- `NEXT_PUBLIC_SEND_EMAIL_ENDPOINT` (varsayilan: `/emails`)

## Rotalar

- `/login`: Login formu
- `/mail`: Inbox ve mesaj detay alani
- `/`: Token varsa `/mail`, yoksa `/login`
