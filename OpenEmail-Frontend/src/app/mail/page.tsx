import { MailClient } from "@/components/mail/mail-client";

export default function MailPage() {
  return (
    <main className="relative min-h-screen overflow-hidden">
      <div className="absolute inset-0 -z-10 bg-[radial-gradient(circle_at_0%_0%,rgba(255,171,64,0.20),transparent_28%),radial-gradient(circle_at_100%_0%,rgba(37,99,235,0.18),transparent_30%),linear-gradient(130deg,#f7f8ff_0%,#fdfaf6_40%,#f5faf8_100%)]" />
      <MailClient />
    </main>
  );
}
