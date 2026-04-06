import { getAccessToken } from "@/lib/auth";

const baseURL =
  process.env.NEXT_PUBLIC_API_BASE_URL?.replace(/\/$/, "") ??
  "https://localhost:7233/api";

type HttpResponse<T> = {
  data: T;
  status: number;
  headers: Headers;
};

type RequestConfig = {
  headers?: HeadersInit;
  timeout?: number;
};

function resolveUrl(path: string): string {
  if (/^https?:\/\//i.test(path)) {
    return path;
  }

  const normalizedPath = path.startsWith("/") ? path : `/${path}`;
  return `${baseURL}${normalizedPath}`;
}

async function request<T>(
  method: "GET" | "POST",
  path: string,
  body?: unknown,
  config?: RequestConfig,
): Promise<HttpResponse<T>> {
  const controller = new AbortController();
  const timeoutMs = config?.timeout ?? 15000;
  const timeoutId = setTimeout(() => controller.abort(), timeoutMs);

  const headers = new Headers(config?.headers);
  const token = getAccessToken();

  if (token) {
    headers.set("Authorization", `Bearer ${token}`);
  }

  if (body !== undefined && body !== null && !headers.has("Content-Type")) {
    headers.set("Content-Type", "application/json");
  }

  try {
    const response = await fetch(resolveUrl(path), {
      method,
      headers,
      body: body !== undefined && body !== null ? JSON.stringify(body) : undefined,
      signal: controller.signal,
      cache: "no-store",
    });

    const contentType = response.headers.get("content-type") ?? "";
    const isJson = contentType.includes("application/json");
    const payload =
      response.status === 204
        ? undefined
        : isJson
          ? await response.json()
          : await response.text();

    if (!response.ok) {
      const fallback = `HTTP ${response.status}`;

      if (payload && typeof payload === "object") {
        const errorBody = payload as { message?: unknown; title?: unknown; error?: unknown };
        const message = errorBody.message ?? errorBody.title ?? errorBody.error;

        throw new Error(typeof message === "string" && message.trim() ? message : fallback);
      }

      throw new Error(typeof payload === "string" && payload.trim() ? payload : fallback);
    }

    return {
      data: payload as T,
      status: response.status,
      headers: response.headers,
    };
  } catch (error) {
    if (error instanceof DOMException && error.name === "AbortError") {
      throw new Error("Istek zaman asimina ugradi.");
    }

    throw error;
  } finally {
    clearTimeout(timeoutId);
  }
}

export const http = {
  get<T>(path: string, config?: RequestConfig): Promise<HttpResponse<T>> {
    return request<T>("GET", path, undefined, config);
  },
  post<T = unknown>(path: string, body?: unknown, config?: RequestConfig): Promise<HttpResponse<T>> {
    return request<T>("POST", path, body, config);
  },
};
