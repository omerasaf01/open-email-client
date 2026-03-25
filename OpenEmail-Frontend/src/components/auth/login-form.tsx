"use client";

import { useState } from "react";
import { useRouter } from "next/navigation";
import { useForm } from "react-hook-form";
import { z } from "zod";
import { zodResolver } from "@hookform/resolvers/zod";
import { useMutation } from "@tanstack/react-query";
import { Mail, ShieldCheck } from "lucide-react";
import { toast } from "sonner";

import { login } from "@/lib/email-api";
import { setAccessToken } from "@/lib/auth";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";

const formSchema = z.object({
  email: z.email("Gecerli bir email adresi girin"),
  password: z.string().min(6, "Sifre en az 6 karakter olmali"),
});

type FormValues = z.infer<typeof formSchema>;

export function LoginForm() {
  const router = useRouter();
  const [showHint, setShowHint] = useState(false);

  const form = useForm<FormValues>({
    resolver: zodResolver(formSchema),
    defaultValues: {
      email: "",
      password: "",
    },
  });

  const loginMutation = useMutation({
    mutationFn: login,
    onSuccess: (data) => {
      setAccessToken(data.accessToken);
      toast.success("Giris basarili");
      router.replace("/mail");
      router.refresh();
    },
    onError: () => {
      toast.error("Giris basarisiz. Bilgilerinizi kontrol edin.");
    },
  });

  const onSubmit = form.handleSubmit((values) => loginMutation.mutate(values));

  return (
    <div className="mx-auto flex w-full max-w-md flex-col gap-6">
      <Card className="border-white/40 bg-white/70 shadow-2xl backdrop-blur-xl">
        <CardHeader className="space-y-3">
          <div className="inline-flex w-fit items-center gap-2 rounded-full bg-black/90 px-3 py-1 text-xs text-white">
            <Mail className="size-3.5" />
            OpenEmail Client
          </div>
          <CardTitle className="text-2xl">Hesabina Giris Yap</CardTitle>
          <CardDescription>
            Kayit olmadan direkt olarak mevcut hesabinizla email kutunuza ulasin.
          </CardDescription>
        </CardHeader>

        <CardContent>
          <form className="space-y-4" onSubmit={onSubmit}>
            <div className="space-y-2">
              <Label htmlFor="email">Email</Label>
              <Input
                id="email"
                placeholder="you@company.com"
                autoComplete="email"
                {...form.register("email")}
              />
              {form.formState.errors.email && (
                <p className="text-xs text-destructive">{form.formState.errors.email.message}</p>
              )}
            </div>

            <div className="space-y-2">
              <Label htmlFor="password">Sifre</Label>
              <Input
                id="password"
                type="password"
                placeholder="••••••••"
                autoComplete="current-password"
                {...form.register("password")}
              />
              {form.formState.errors.password && (
                <p className="text-xs text-destructive">{form.formState.errors.password.message}</p>
              )}
            </div>

            <Button className="w-full" size="lg" type="submit" disabled={loginMutation.isPending}>
              {loginMutation.isPending ? "Giris yapiliyor..." : "Giris Yap"}
            </Button>

            <button
              type="button"
              onClick={() => setShowHint((prev) => !prev)}
              className="inline-flex items-center gap-2 text-xs text-muted-foreground hover:text-foreground"
            >
              <ShieldCheck className="size-3.5" />
              API endpoint notu
            </button>

            {showHint && (
              <p className="rounded-lg border bg-muted/50 p-3 text-xs text-muted-foreground">
                Giris istegi varsayilan olarak /auth/login endpointine gider. Farkli ise NEXT_PUBLIC_LOGIN_ENDPOINT
                degiskenini ayarlayin.
              </p>
            )}
          </form>
        </CardContent>
      </Card>
    </div>
  );
}
