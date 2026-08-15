export interface Customer {
  id: string;
  name: string;
  customerNumber?: string;
  firstName?: string;
  lastName?: string;
  email?: string;
  phoneNumber?: string;
}

export interface Department {
  id: string;
  name: string;
  code?: string;
  description?: string;
  status: number;
}

export interface DepartmentCreateRequest {
  name: string;
  code: string;
  description?: string;
  status: number;
}

export interface DepartmentUpdateRequest {
  id: string;
  name?: string;
  code?: string;
  description?: string;
  status?: number;
}

export interface Category {
  id: string;
  name: string;
  description?: string;
  isActive?: boolean;
  subCategories?: Subcategory[];
}

export interface Subcategory {
  id: string;
  name: string;
  description?: string;
  isActive?: boolean;
}

export interface Ticket {
  id: string;
  ticketNumber?: string;

  subject: string;
  description: string;

  status: string;
  priority: number;

  customerId?: string;
  customer?: Customer;

  categoryId?: string;
  category?: Category;

  departmentId?: string;
  department?: Department;

  assignedUserId?: string;

  createdAt?: string;
  updatedAt?: string;

  slaStartedAt?: string;
  slaResponseDueAt?: string;
  slaResolutionDueAt?: string;
  isSlaPaused?: boolean;
  slaPausedAt?: string;
  totalSlaPausedDuration?: string;
}

export interface TicketSearchResult {
  ticketId: string;
  ticketNumber: string;

  customerId: string;
  customerName: string;

  departmentId: string;
  categoryId: string;
  subCategoryId?: string;

  subject: string;
  description: string;

  priority: string;
  status: string;

  occurredOn: string;

  assignedUserId?: string;

  slaStartedAt: string;
  slaResponseDueAt: string;
  slaResolutionDueAt: string;

  isSlaPaused: boolean;
  slaPausedAt?: string;
  totalSlaPausedDuration: string;

  isSlaBreached: boolean;
}

export interface TicketComment {
  id: string;
  ticketId: string;
  content: string;

  author?: string;
  userId?: string;
  userName?: string;

  type?: number;
  isInternal?: boolean;

  createdAt?: string;
  isDeleted?: boolean;
}

export interface TicketEvent {
  id: string;
  ticketId: string;
  eventType?: string;
  description?: string;
  userName?: string;
  createdAt?: string;
}

export interface TicketCreateRequest {
  customerId: string;
  departmentId: string;
  categoryId: string;
  subCategoryId?: string;

  subject: string;
  description: string;

  priority: number;
}

export interface TicketUpdateRequest {
  subject?: string;
  description?: string;
  status?: string;
  priority?: number;
  categoryId?: string;
  departmentId?: string;
  assignedUserId?: string;
}

export interface TicketSearchParams {
  search?: string;
  status?: string;
  priority?: string;
  categoryId?: string;
  departmentId?: string;
  assignedUserId?: string;
  slaBreached?: boolean;

  page?: number;
  pageSize?: number;
}

export interface Projection {
  [key: string]: unknown;
}

export interface SlaDashboard {
  [key: string]: unknown;
}

export interface SlaItem {
  id?: string;
  name?: string;
  status?: string;
  value?: number;
  count?: number;
  [key: string]: unknown;
}

