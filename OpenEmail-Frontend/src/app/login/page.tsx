import { LoginForm } from "@/components/auth/login-form";

export default function LoginPage() {
  return (
    <main className="relative flex min-h-screen items-center justify-center overflow-hidden p-4">
      <div className="absolute inset-0 -z-10 bg-[radial-gradient(circle_at_10%_20%,rgba(255,140,77,0.28),transparent_35%),radial-gradient(circle_at_88%_82%,rgba(26,140,255,0.24),transparent_38%),linear-gradient(140deg,#fdf8f3_0%,#f6fafc_55%,#fffdf7_100%)]" />
      <div className="absolute top-10 right-10 -z-10 h-48 w-48 rounded-full border border-black/10 bg-white/50 blur-2xl" />
      <div className="absolute bottom-10 left-10 -z-10 h-56 w-56 rounded-full border border-black/10 bg-white/40 blur-2xl" />

      <LoginForm />
    </main>
  );
}
