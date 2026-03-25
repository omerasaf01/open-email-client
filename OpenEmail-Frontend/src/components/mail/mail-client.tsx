"use client";

import { useMemo, useState } from "react";
import { useRouter } from "next/navigation";
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import {
  Inbox,
  LogOut,
  MailPlus,
  RefreshCw,
  Search,
  UserRound,
} from "lucide-react";
import { toast } from "sonner";

import { clearAccessToken } from "@/lib/auth";
import { getEmailById, getInbox, sendEmail } from "@/lib/email-api";
import type { SendEmailInput } from "@/lib/types";

import { Avatar, AvatarFallback } from "@/components/ui/avatar";
import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuLabel,
  DropdownMenuSeparator,
  DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu";
import { Input } from "@/components/ui/input";
import { ScrollArea } from "@/components/ui/scroll-area";
import { Separator } from "@/components/ui/separator";
import { Skeleton } from "@/components/ui/skeleton";

import { ComposeEmailDialog } from "@/components/mail/compose-email-dialog";

export function MailClient() {
  const router = useRouter();
  const queryClient = useQueryClient();

  const [search, setSearch] = useState("");
  const [selectedEmailId, setSelectedEmailId] = useState<string | null>(null);
  const [composeOpen, setComposeOpen] = useState(false);

  const inboxQuery = useQuery({
    queryKey: ["emails", "inbox"],
    queryFn: getInbox,
  });

  const selectedEmailQuery = useQuery({
    queryKey: ["emails", "detail", selectedEmailId],
    queryFn: () => getEmailById(selectedEmailId ?? ""),
    enabled: Boolean(selectedEmailId),
  });

  const sendMutation = useMutation({
    mutationFn: sendEmail,
    onSuccess: async () => {
      toast.success("Email gonderildi");
      setComposeOpen(false);
      await queryClient.invalidateQueries({ queryKey: ["emails"] });
    },
    onError: () => toast.error("Email gonderilemedi"),
  });

  const filteredEmails = useMemo(() => {
    const emails = inboxQuery.data ?? [];
    const q = search.trim().toLowerCase();
    if (!q) {
      return emails;
    }

    return emails.filter((mail) => {
      const haystack = `${mail.from} ${mail.subject} ${mail.snippet}`.toLowerCase();
      return haystack.includes(q);
    });
  }, [inboxQuery.data, search]);

  const logout = () => {
    clearAccessToken();
    router.replace("/login");
    router.refresh();
  };

  const onComposeSubmit = (values: SendEmailInput) => {
    sendMutation.mutate(values);
  };

  const isRefreshing = inboxQuery.isRefetching;

  return (
    <>
      <div className="mx-auto flex h-screen w-full max-w-[1400px] flex-col overflow-hidden p-4 md:p-6">
        <header className="mb-4 flex flex-wrap items-center justify-between gap-3 rounded-2xl border border-black/10 bg-white/70 p-3 shadow-lg backdrop-blur-xl">
          <div className="inline-flex items-center gap-2 text-lg font-semibold tracking-tight">
            <Inbox className="size-5" />
            OpenEmail Inbox
            <Badge variant="outline">MVP</Badge>
          </div>

          <div className="flex items-center gap-2">
            <Button variant="outline" onClick={() => inboxQuery.refetch()} disabled={isRefreshing}>
              <RefreshCw className={`size-4 ${isRefreshing ? "animate-spin" : ""}`} />
              {isRefreshing ? "Yenileniyor..." : "Yenile"}
            </Button>
            <Button onClick={() => setComposeOpen(true)}>
              <MailPlus className="size-4" />
              Yeni
            </Button>

            <DropdownMenu>
              <DropdownMenuTrigger asChild>
                <Button variant="outline" size="icon">
                  <Avatar className="size-6">
                    <AvatarFallback>ME</AvatarFallback>
                  </Avatar>
                </Button>
              </DropdownMenuTrigger>

              <DropdownMenuContent align="end" className="w-44">
                <DropdownMenuLabel>Hesap</DropdownMenuLabel>
                <DropdownMenuSeparator />
                <DropdownMenuItem onClick={logout}>
                  <LogOut className="size-4" />
                  Cikis Yap
                </DropdownMenuItem>
              </DropdownMenuContent>
            </DropdownMenu>
          </div>
        </header>

        <div className="grid min-h-0 flex-1 gap-4 lg:grid-cols-[340px_minmax(0,1fr)]">
          <Card className="min-h-0 min-w-0 border-black/10 bg-white/70 backdrop-blur-xl">
            <CardHeader className="space-y-3">
              <CardTitle className="text-base">Inbox</CardTitle>

              <div className="relative">
                <Search className="pointer-events-none absolute top-1/2 left-3 size-4 -translate-y-1/2 text-muted-foreground" />
                <Input
                  className="pl-9"
                  placeholder="Maillerde ara"
                  value={search}
                  onChange={(event) => setSearch(event.target.value)}
                />
              </div>
            </CardHeader>

            <Separator />

            <CardContent className="min-h-0 min-w-0 flex-1 pt-4">
              <ScrollArea className="h-full pr-3">
                <div className="w-full space-y-2 pr-1">
                  {inboxQuery.isLoading && (
                    <div className="space-y-2">
                      <Skeleton className="h-20 w-full" />
                      <Skeleton className="h-20 w-full" />
                      <Skeleton className="h-20 w-full" />
                    </div>
                  )}

                  {inboxQuery.isError && (
                    <p className="rounded-lg border border-destructive/30 bg-destructive/10 p-3 text-sm text-destructive">
                      Inbox verileri alinamadi. API endpointlerini kontrol edin.
                    </p>
                  )}

                  {!inboxQuery.isLoading && filteredEmails.length === 0 && (
                    <p className="rounded-lg border bg-muted/50 p-3 text-sm text-muted-foreground">
                      Gosterilecek email yok.
                    </p>
                  )}

                  {filteredEmails.map((mail) => (
                    <button
                      key={mail.id}
                      type="button"
                      onClick={() => setSelectedEmailId(mail.id)}
                      className="w-full max-w-full min-w-0 overflow-hidden rounded-xl border border-transparent bg-muted/30 p-3 text-left transition hover:border-black/10 hover:bg-muted/60 hover:cursor-pointer"
                    >
                      <div className="mb-1 flex min-w-0 items-center justify-between gap-2">
                        <p className="min-w-0 flex-1 truncate break-all text-sm font-medium">{mail.from}</p>
                        {!mail.isRead && (
                          <Badge
                            variant="default"
                            className="px-2 py-0.5 text-[10px] font-bold tracking-wide uppercase shadow-sm"
                          >
                            Okunmadi
                          </Badge>
                        )}
                      </div>
                      <p className="truncate break-all text-sm font-semibold">{mail.subject}</p>
                      <p className="truncate break-all text-xs text-muted-foreground">{mail.snippet}</p>
                    </button>
                  ))}
                </div>
              </ScrollArea>
            </CardContent>
          </Card>

          <Card className="border-black/10 bg-white/70 backdrop-blur-xl">
            <CardHeader>
              <CardTitle className="text-base">Mesaj Detayi</CardTitle>
            </CardHeader>
            <Separator />
            <CardContent className="space-y-4 py-4">
              {!selectedEmailId && (
                <div className="flex min-h-[320px] flex-col items-center justify-center gap-3 rounded-xl border border-dashed bg-muted/40 text-center">
                  <UserRound className="size-9 text-muted-foreground" />
                  <p className="text-sm text-muted-foreground">Detay goruntulemek icin soldan bir email secin.</p>
                </div>
              )}

              {selectedEmailId && selectedEmailQuery.isLoading && (
                <div className="space-y-3">
                  <Skeleton className="h-8 w-3/4" />
                  <Skeleton className="h-4 w-1/2" />
                  <Skeleton className="h-60 w-full" />
                </div>
              )}

              {selectedEmailQuery.data && (
                <article className="space-y-3">
                  <h2 className="text-xl font-semibold tracking-tight">{selectedEmailQuery.data.subject}</h2>
                  <div className="text-xs text-muted-foreground">
                    <p>Kimden: {selectedEmailQuery.data.from}</p>
                    <p>Kime: {selectedEmailQuery.data.to}</p>
                    <p>Tarih: {new Date(selectedEmailQuery.data.sentAt).toLocaleString("tr-TR")}</p>
                  </div>
                  <Separator />
                  <p className="leading-7 whitespace-pre-wrap">{selectedEmailQuery.data.body}</p>
                </article>
              )}
            </CardContent>
          </Card>
        </div>
      </div>

      <ComposeEmailDialog
        open={composeOpen}
        pending={sendMutation.isPending}
        onOpenChange={setComposeOpen}
        onSubmit={onComposeSubmit}
      />
    </>
  );
}
