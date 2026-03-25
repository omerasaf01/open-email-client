"use client";

import { useEffect } from "react";
import { useForm } from "react-hook-form";
import { z } from "zod";
import { zodResolver } from "@hookform/resolvers/zod";

import { Button } from "@/components/ui/button";
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Textarea } from "@/components/ui/textarea";

const composeSchema = z.object({
  to: z.email("Gecerli bir alici email girin"),
  subject: z.string().min(1, "Konu gerekli"),
  body: z.string().min(3, "Mesaj metni gerekli"),
});

type ComposeValues = z.infer<typeof composeSchema>;

type ComposeEmailDialogProps = {
  open: boolean;
  pending: boolean;
  onOpenChange: (open: boolean) => void;
  onSubmit: (values: ComposeValues) => void;
};

export function ComposeEmailDialog({
  open,
  pending,
  onOpenChange,
  onSubmit,
}: ComposeEmailDialogProps) {
  const form = useForm<ComposeValues>({
    resolver: zodResolver(composeSchema),
    defaultValues: {
      to: "",
      subject: "",
      body: "",
    },
  });

  useEffect(() => {
    if (!open) {
      form.reset();
    }
  }, [form, open]);

  const submit = form.handleSubmit(onSubmit);

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="sm:max-w-xl">
        <DialogHeader>
          <DialogTitle>Yeni Email Yaz</DialogTitle>
          <DialogDescription>Yeni bir mesaj hazirlayip backend API uzerinden gonderebilirsiniz.</DialogDescription>
        </DialogHeader>

        <form className="space-y-4" onSubmit={submit}>
          <div className="space-y-2">
            <Label htmlFor="to">Kime</Label>
            <Input id="to" placeholder="recipient@company.com" {...form.register("to")} />
            {form.formState.errors.to && (
              <p className="text-xs text-destructive">{form.formState.errors.to.message}</p>
            )}
          </div>

          <div className="space-y-2">
            <Label htmlFor="subject">Konu</Label>
            <Input id="subject" placeholder="Konu satiri" {...form.register("subject")} />
            {form.formState.errors.subject && (
              <p className="text-xs text-destructive">{form.formState.errors.subject.message}</p>
            )}
          </div>

          <div className="space-y-2">
            <Label htmlFor="body">Mesaj</Label>
            <Textarea id="body" rows={8} placeholder="Mesajinizi yazin..." {...form.register("body")} />
            {form.formState.errors.body && (
              <p className="text-xs text-destructive">{form.formState.errors.body.message}</p>
            )}
          </div>

          <DialogFooter>
            <Button type="button" variant="outline" onClick={() => onOpenChange(false)}>
              Vazgec
            </Button>
            <Button type="submit" disabled={pending}>
              {pending ? "Gonderiliyor..." : "Gonder"}
            </Button>
          </DialogFooter>
        </form>
      </DialogContent>
    </Dialog>
  );
}
