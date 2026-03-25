import Cookies from "js-cookie";
import { AUTH_COOKIE_NAME } from "@/lib/constants";

export function getAccessToken(): string | undefined {
  if (typeof window === "undefined") {
    return undefined;
  }

  return Cookies.get(AUTH_COOKIE_NAME);
}

export function setAccessToken(token: string): void {
  Cookies.set(AUTH_COOKIE_NAME, token, {
    expires: 1,
    sameSite: "lax",
  });
}

export function clearAccessToken(): void {
  Cookies.remove(AUTH_COOKIE_NAME);
}
