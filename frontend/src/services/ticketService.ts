import api from "./api";

import type {
  Ticket,
  TicketComment,
  TicketCreateRequest,
  TicketEvent,
  TicketSearchParams,
  TicketSearchResult,
  TicketUpdateRequest,
  Projection,
  SlaDashboard,
} from "../types";

const PRIORITY_NUMBER_MAP: Record<string, string> = {
  Low: "1",
  Medium: "2",
  High: "3",
  Critical: "4",
};

const normalizePriority = (
  priority: unknown
): string => {
  if (typeof priority === "number") {
    return String(priority);
  }

  if (typeof priority === "string") {
    const trimmed = priority.trim();

    if (
      trimmed === "1" ||
      trimmed === "2" ||
      trimmed === "3" ||
      trimmed === "4"
    ) {
      return trimmed;
    }

    return PRIORITY_NUMBER_MAP[trimmed] ?? trimmed;
  }

  return "";
};

export const getTickets = async (
  params?: TicketSearchParams
): Promise<TicketSearchResult[]> => {
  /*
   * Priority filtresini frontend tarafında
   * uyguluyoruz.
   *
   * Bunun nedeni eski Elasticsearch kayıtlarında
   * priority alanının farklı formatlarda bulunabilmesi:
   *
   * 1 / 2 / 3 / 4
   * Low / Medium / High / Critical
   *
   * Böylece backend'in priority filtresinin
   * veri formatına bağlı kalmıyoruz.
   */
  const response = await api.get(
    "/tickets/search",
    {
      params: {
        q: params?.search || undefined,

        status:
          params?.status || undefined,

        categoryId:
          params?.categoryId || undefined,

        departmentId:
          params?.departmentId || undefined,

        assignedUserId:
          params?.assignedUserId || undefined,

        slaBreached:
          params?.slaBreached !== undefined
            ? params.slaBreached
            : undefined,
      },
    }
  );

  let tickets: TicketSearchResult[] =
    Array.isArray(response.data)
      ? response.data
      : [];

  /*
   * Priority seçilmişse frontend tarafında filtrele.
   */
  if (
    params?.priority !== undefined &&
    params.priority !== ""
  ) {
    const selectedPriority =
      String(params.priority);

    tickets = tickets.filter(
      (ticket) =>
        normalizePriority(
          ticket.priority
        ) === selectedPriority
    );
  }

  return tickets;
};

export const getTicketById = async (
  id: string | number
): Promise<Ticket> => {
  const response = await api.get(
    `/tickets/${id}`
  );

  return response.data;
};

export const createTicket = async (
  data: TicketCreateRequest
): Promise<Ticket> => {
  const response = await api.post(
    "/tickets",
    data
  );

  return response.data;
};

export const updateTicket = async (
  id: string | number,
  data: TicketUpdateRequest
): Promise<Ticket> => {
  const response = await api.put(
    `/tickets/${id}`,
    data
  );

  return response.data;
};

export const getTicketComments = async (
  ticketId: string | number
): Promise<TicketComment[]> => {
  const response = await api.get(
    `/tickets/${ticketId}/comments`
  );

  return response.data.map(
    (comment: any) => ({
      id: comment.id,
      ticketId: comment.ticketId,
      userId: comment.userId,

      author:
        comment.author ??
        comment.authorName ??
        "Kullanıcı",

      userName:
        comment.author ??
        comment.authorName ??
        "Kullanıcı",

      content: comment.content,

      type: comment.type,

      isInternal:
        comment.isInternal ??
        comment.type === 2,

      createdAt: comment.createdAt,

      isDeleted: comment.isDeleted,
    })
  );
};

export const addTicketComment = async (
  ticketId: string | number,
  content: string,
  isInternal: boolean
): Promise<string> => {
  const author =
    localStorage.getItem("name") ||
    localStorage.getItem("username") ||
    "Kullanıcı";

  const response = await api.post(
    `/tickets/${ticketId}/comments`,
    {
      content,
      author,
      type: isInternal ? 2 : 1,
    }
  );

  return response.data;
};

export const deleteTicketComment = async (
  ticketId: string | number,
  commentId: string | number
): Promise<void> => {
  await api.delete(
    `/tickets/${ticketId}/comments/${commentId}`
  );
};

export const getTicketEvents = async (
  ticketId: string | number
): Promise<TicketEvent[]> => {
  const response = await api.get(
    `/tickets/${ticketId}/events`
  );

  return response.data;
};

/*
 * Backend TicketStatus enum:
 *
 * Open = 1
 * InProgress = 2
 * WaitingForCustomer = 3
 * Resolved = 4
 * Closed = 5
 */

export const changeTicketStatus = async (
  ticketId: string | number,
  status: string
): Promise<void> => {
  const validStatuses = [
    "Open",
    "InProgress",
    "WaitingForCustomer",
    "Resolved",
    "Closed",
  ];

  if (!validStatuses.includes(status)) {
    throw new Error(
      `Geçersiz talep durumu: ${status}`
    );
  }

  await api.post(
    `/tickets/${ticketId}/status`,
    {
      status,
    }
  );
};

/*
 * Backend TicketPriority enum:
 *
 * Low = 1
 * Medium = 2
 * High = 3
 * Critical = 4
 */

export const changeTicketPriority = async (
  ticketId: string | number,
  priority: number
): Promise<void> => {
  if (
    priority < 1 ||
    priority > 4
  ) {
    throw new Error(
      `Geçersiz öncelik değeri: ${priority}`
    );
  }

  await api.post(
    `/tickets/${ticketId}/priority`,
    priority
  );
};

export const assignTicket = async (
  ticketId: string | number,
  assignedUserId: string
): Promise<void> => {
  await api.post(
    `/tickets/${ticketId}/assign/${assignedUserId}`
  );
};

export const transferTicket = async (
  ticketId: string | number,
  departmentId: string
): Promise<void> => {
  await api.post(
    `/tickets/${ticketId}/transfer/${departmentId}`
  );
};

export const getProjection =
  async (): Promise<Projection> => {
    const response = await api.get(
      "/tickets/projection"
    );

    return response.data;
  };

export const rebuildProjection =
  async (): Promise<void> => {
    await api.post(
      "/tickets/projection/rebuild"
    );
  };

export const getSlaDashboard =
  async (): Promise<SlaDashboard> => {
    const response = await api.get(
      "/tickets/sla-dashboard"
    );

    return response.data;
  };

export default {
  getTickets,
  getTicketById,
  createTicket,
  updateTicket,
  getTicketComments,
  addTicketComment,
  deleteTicketComment,
  getTicketEvents,
  changeTicketPriority,
  changeTicketStatus,
  assignTicket,
  transferTicket,
  getProjection,
  rebuildProjection,
  getSlaDashboard,
};