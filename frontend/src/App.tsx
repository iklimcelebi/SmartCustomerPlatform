import { useEffect, useMemo, useState } from "react";
import "./App.css";

type DepartmentStatus = 0 | 1;

interface Department {
  id: string;
  code: string;
  name: string;
  description: string;
  status: DepartmentStatus;
}

interface Customer {
  id: string;
  tckNo: string;
  firstName: string;
  lastName: string;
  email: string;
  birthDate: string;
  status: number;
}

interface Category {
  id: string;
  code: string;
  name: string;
  description: string;
  isActive: boolean;
}

interface SubCategory {
  id: string;
  code: string;
  name: string;
  description: string;
  isActive: boolean;
  categoryId: string;
}

interface TicketForm {
  customerId: string;
  departmentId: string;
  categoryId: string;
  subCategoryId: string;
  subject: string;
  description: string;
  priority: number;
}

interface Ticket {
  id: string;
  ticketNumber: string;
  customerId: string;
  departmentId: string;
  assignedUserId?: string | null;
  categoryId: string;
  subCategoryId?: string | null;
  subject: string;
  description: string;
  status: number;
  priority: number;
  slaStartedAt?: string | null;
  slaResponseDueAt?: string | null;
  slaResolutionDueAt?: string | null;
  isSlaPaused: boolean;
  slaPausedAt?: string | null;
  totalSlaPausedDuration?: string | null;
}

interface Comment {
  id?: string;
  commentId?: string;
  content?: string;
  text?: string;
  author?: string;
  createdAt?: string;
  occurredOn?: string;
  date?: string;
  type?: number;
}

const API_BASE = "http://localhost:5185/api";

function App() {
  const [page, setPage] = useState<
    "departments" | "customers" | "tickets" | "create-ticket" | "ticket-detail"
  >("departments");

  const [selectedTicketId, setSelectedTicketId] = useState<string | null>(
    null
  );

  const [departments, setDepartments] = useState<Department[]>([]);
  const [customers, setCustomers] = useState<Customer[]>([]);
  const [categories, setCategories] = useState<Category[]>([]);
  const [subCategories, setSubCategories] = useState<SubCategory[]>([]);
  const [tickets, setTickets] = useState<Ticket[]>([]);

  const [loading, setLoading] = useState(false);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState("");
  const [successMessage, setSuccessMessage] = useState("");

  const [customerSearch, setCustomerSearch] = useState("");
  const [customerDropdownOpen, setCustomerDropdownOpen] = useState(false);

  const [ticketSearch, setTicketSearch] = useState("");
  const [ticketStatusFilter, setTicketStatusFilter] = useState("all");
  const [ticketPriorityFilter, setTicketPriorityFilter] = useState("all");

  const [ticketForm, setTicketForm] = useState<TicketForm>({
    customerId: "",
    departmentId: "",
    categoryId: "",
    subCategoryId: "",
    subject: "",
    description: "",
    priority: 1,
  });

  const loadDepartments = async () => {
    try {
      setLoading(true);
      setError("");

      const response = await fetch(`${API_BASE}/Departments`);

      if (!response.ok) {
        throw new Error("Departmanlar alınamadı.");
      }

      setDepartments(await response.json());
    } catch (err) {
      setError(
        err instanceof Error ? err.message : "Departmanlar alınamadı."
      );
    } finally {
      setLoading(false);
    }
  };

  const loadCustomers = async () => {
    try {
      setLoading(true);
      setError("");

      const response = await fetch(`${API_BASE}/Customers`);

      if (!response.ok) {
        throw new Error("Müşteriler alınamadı.");
      }

      setCustomers(await response.json());
    } catch (err) {
      setError(
        err instanceof Error ? err.message : "Müşteriler alınamadı."
      );
    } finally {
      setLoading(false);
    }
  };

  const loadCategories = async () => {
    try {
      setError("");

      const response = await fetch(`${API_BASE}/TicketCategories`);

      if (!response.ok) {
        throw new Error("Kategoriler alınamadı.");
      }

      setCategories(await response.json());
    } catch (err) {
      setError(
        err instanceof Error ? err.message : "Kategoriler alınamadı."
      );
    }
  };

  const loadSubCategories = async (categoryId: string) => {
    if (!categoryId) {
      setSubCategories([]);
      return;
    }

    try {
      setError("");

      const response = await fetch(
        `${API_BASE}/TicketSubCategories/category/${categoryId}`
      );

      if (!response.ok) {
        throw new Error("Alt kategoriler alınamadı.");
      }

      const data: SubCategory[] = await response.json();

      const uniqueSubCategories = Array.from(
        new Map(data.map((item) => [item.id, item])).values()
      );

      setSubCategories(uniqueSubCategories);
    } catch (err) {
      setError(
        err instanceof Error ? err.message : "Alt kategoriler alınamadı."
      );
    }
  };

  const loadTickets = async () => {
    try {
      setLoading(true);
      setError("");

      const response = await fetch(`${API_BASE}/Tickets`);

      if (!response.ok) {
        throw new Error("Talepler alınamadı.");
      }

      setTickets(await response.json());
    } catch (err) {
      setError(err instanceof Error ? err.message : "Talepler alınamadı.");
    } finally {
      setLoading(false);
    }
  };

  const searchTickets = async () => {
    try {
      setLoading(true);
      setError("");

      const params = new URLSearchParams();

      if (ticketSearch.trim()) {
        params.set("q", ticketSearch.trim());
      }

      if (ticketStatusFilter !== "all") {
        params.set("status", ticketStatusFilter);
      }

      if (ticketPriorityFilter !== "all") {
        params.set("priority", ticketPriorityFilter);
      }

      const query = params.toString();

      const response = await fetch(
        `${API_BASE}/Tickets/search${query ? `?${query}` : ""}`
      );

      if (!response.ok) {
        throw new Error("Elasticsearch araması başarısız.");
      }

      const results = await response.json();

      setTickets(
        results.map((item: any) => ({
          id: item.ticketId,
          ticketNumber: item.ticketNumber,
          customerId: item.customerId,
          departmentId: item.departmentId,
          assignedUserId: item.assignedUserId ?? null,
          categoryId: item.categoryId,
          subCategoryId: item.subCategoryId ?? null,
          subject: item.subject,
          description: item.description ?? "",

          status:
            item.status === "Open"
              ? 1
              : item.status === "InProgress"
              ? 2
              : item.status === "Resolved"
              ? 3
              : item.status === "Closed"
              ? 4
              : 1,

          priority:
            item.priority === "Low"
              ? 1
              : item.priority === "Medium"
              ? 2
              : item.priority === "High"
              ? 3
              : item.priority === "Critical"
              ? 4
              : 1,

          slaStartedAt: item.slaStartedAt ?? null,
          slaResponseDueAt: item.slaResponseDueAt ?? null,
          slaResolutionDueAt: item.slaResolutionDueAt ?? null,
          isSlaPaused: item.isSlaPaused ?? false,
          slaPausedAt: item.slaPausedAt ?? null,
          totalSlaPausedDuration: item.totalSlaPausedDuration ?? null,
        }))
      );
    } catch (err) {
      setError(
        err instanceof Error
          ? err.message
          : "Elasticsearch araması başarısız."
      );
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    loadDepartments();
  }, []);

  useEffect(() => {
    if (page === "customers") {
      loadCustomers();
    }

    if (page === "tickets") {
      loadTickets();
      loadCustomers();
      loadDepartments();
      loadCategories();
    }

    if (page === "create-ticket") {
      loadCustomers();
      loadDepartments();
      loadCategories();
    }
  }, [page]);

  const handleCategoryChange = async (categoryId: string) => {
    setTicketForm((current) => ({
      ...current,
      categoryId,
      subCategoryId: "",
    }));

    await loadSubCategories(categoryId);
  };

  const selectedCustomer = useMemo(
    () => customers.find((customer) => customer.id === ticketForm.customerId),
    [customers, ticketForm.customerId]
  );

  const filteredTicketCustomers = useMemo(() => {
    const search = customerSearch.trim().toLowerCase();

    if (!search) {
      return [];
    }

    return customers
      .filter((customer) => {
        const fullName =
          `${customer.firstName} ${customer.lastName}`.toLowerCase();

        return (
          fullName.includes(search) ||
          customer.tckNo.toLowerCase().includes(search) ||
          customer.email.toLowerCase().includes(search)
        );
      })
      .slice(0, 10);
  }, [customers, customerSearch]);

  const handleCustomerSelect = (customer: Customer) => {
    setTicketForm((current) => ({
      ...current,
      customerId: customer.id,
    }));

    setCustomerSearch(`${customer.firstName} ${customer.lastName}`);
    setCustomerDropdownOpen(false);
  };

  const handleCreateTicket = async (
    event: React.FormEvent<HTMLFormElement>
  ) => {
    event.preventDefault();

    setError("");
    setSuccessMessage("");

    if (!ticketForm.customerId) {
      setError("Lütfen bir müşteri seçin.");
      return;
    }

    if (!ticketForm.departmentId) {
      setError("Lütfen bir departman seçin.");
      return;
    }

    if (!ticketForm.categoryId) {
      setError("Lütfen bir kategori seçin.");
      return;
    }

    if (!ticketForm.subject.trim()) {
      setError("Lütfen talep konusu girin.");
      return;
    }

    if (!ticketForm.description.trim()) {
      setError("Lütfen talep açıklaması girin.");
      return;
    }

    try {
      setSaving(true);

      const response = await fetch(`${API_BASE}/Tickets`, {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify({
          customerId: ticketForm.customerId,
          departmentId: ticketForm.departmentId,
          categoryId: ticketForm.categoryId,
          subCategoryId: ticketForm.subCategoryId || null,
          subject: ticketForm.subject.trim(),
          description: ticketForm.description.trim(),
          priority: ticketForm.priority,
        }),
      });

      if (!response.ok) {
        const message = await response.text();
        throw new Error(message || "Talep oluşturulamadı.");
      }

      const ticketId = await response.json();

      setSuccessMessage(
        `Talep başarıyla oluşturuldu. Talep ID: ${ticketId}`
      );

      setTicketForm({
        customerId: "",
        departmentId: "",
        categoryId: "",
        subCategoryId: "",
        subject: "",
        description: "",
        priority: 1,
      });

      setCustomerSearch("");
      setCustomerDropdownOpen(false);
      setSubCategories([]);
    } catch (err) {
      setError(
        err instanceof Error
          ? err.message
          : "Talep oluşturulurken bir hata oluştu."
      );
    } finally {
      setSaving(false);
    }
  };

  const getCustomerName = (customerId: string) => {
    const customer = customers.find((item) => item.id === customerId);

    if (!customer) {
      return "Bilinmiyor";
    }

    return `${customer.firstName} ${customer.lastName}`;
  };

  const getDepartmentName = (departmentId: string) => {
    const department = departments.find(
      (item) => item.id === departmentId
    );

    return department?.name ?? "Bilinmiyor";
  };

  const getCategoryName = (categoryId: string) => {
    const category = categories.find((item) => item.id === categoryId);

    return category?.name ?? "Bilinmiyor";
  };

  const getTicketStatus = (status: number) => {
    switch (status) {
      case 1:
        return {
          text: "Açık",
          className: "active",
        };

      case 2:
        return {
          text: "Devam Ediyor",
          className: "warning",
        };

      case 3:
        return {
          text: "Çözüldü",
          className: "success",
        };

      case 4:
        return {
          text: "Kapalı",
          className: "passive",
        };

      default:
        return {
          text: `Durum ${status}`,
          className: "passive",
        };
    }
  };

  const getPriority = (priority: number) => {
    switch (priority) {
      case 1:
        return {
          text: "Düşük",
          className: "low",
        };

      case 2:
        return {
          text: "Orta",
          className: "medium",
        };

      case 3:
        return {
          text: "Yüksek",
          className: "high",
        };

      case 4:
        return {
          text: "Kritik",
          className: "critical",
        };

      default:
        return {
          text: `Öncelik ${priority}`,
          className: "low",
        };
    }
  };

  const formatDate = (date?: string | null) => {
    if (!date || date.startsWith("0001-01-01")) {
      return "-";
    }

    const parsedDate = new Date(date);

    if (Number.isNaN(parsedDate.getTime())) {
      return "-";
    }

    return parsedDate.toLocaleString("tr-TR");
  };

  const filteredTickets = useMemo(() => {
    const search = ticketSearch.trim().toLowerCase();

    return tickets.filter((ticket) => {
      const customerName = getCustomerName(ticket.customerId);
      const departmentName = getDepartmentName(ticket.departmentId);
      const categoryName = getCategoryName(ticket.categoryId);

      const matchesSearch =
        !search ||
        ticket.ticketNumber.toLowerCase().includes(search) ||
        ticket.subject.toLowerCase().includes(search) ||
        ticket.description.toLowerCase().includes(search) ||
        customerName.toLowerCase().includes(search) ||
        departmentName.toLowerCase().includes(search) ||
        categoryName.toLowerCase().includes(search);

      const matchesStatus =
        ticketStatusFilter === "all" ||
        ticket.status.toString() === ticketStatusFilter;

      const matchesPriority =
        ticketPriorityFilter === "all" ||
        ticket.priority.toString() === ticketPriorityFilter;

      return matchesSearch && matchesStatus && matchesPriority;
    });
  }, [
    tickets,
    ticketSearch,
    ticketStatusFilter,
    ticketPriorityFilter,
    customers,
    departments,
    categories,
  ]);

  return (
    <div className="app">
      <header className="topbar">
        <div>
          <div className="brand">SmartCustomerPlatform</div>

          <div className="subtitle">
            Müşteri Destek Yönetim Sistemi
          </div>
        </div>

        <div className="topbar-user">
          <span className="avatar">İ</span>

          <div>
            <strong>İklim</strong>
            <small>Yönetici</small>
          </div>
        </div>
      </header>

      <div className="layout">
        <aside className="sidebar">
          <div className="sidebar-title">Yönetim</div>

          <button
            className={
              page === "departments"
                ? "nav-button active"
                : "nav-button"
            }
            onClick={() => {
              setPage("departments");
              setError("");
              setSuccessMessage("");
            }}
          >
            Departmanlar
          </button>

          <button
            className={
              page === "customers"
                ? "nav-button active"
                : "nav-button"
            }
            onClick={() => {
              setPage("customers");
              setError("");
              setSuccessMessage("");
            }}
          >
            Müşteriler
          </button>

          <div className="sidebar-title ticket-title">
            Talepler
          </div>

          <button
            className={
              page === "tickets"
                ? "nav-button active"
                : "nav-button"
            }
            onClick={() => {
              setPage("tickets");
              setError("");
              setSuccessMessage("");
            }}
          >
            Tüm Talepler
          </button>

          <button
            className={
              page === "create-ticket"
                ? "nav-button active"
                : "nav-button"
            }
            onClick={() => {
              setPage("create-ticket");
              setError("");
              setSuccessMessage("");
            }}
          >
            Yeni Talep
          </button>
        </aside>

        <main className="container">
          {error && (
            <div className="error">
              <strong>Hata:</strong> {error}
            </div>
          )}

          {successMessage && (
            <div className="success-message">
              <strong>Başarılı</strong>
              <span>{successMessage}</span>
            </div>
          )}

          {page === "departments" && (
            <DepartmentPage
              departments={departments}
              loading={loading}
              onRefresh={loadDepartments}
            />
          )}

          {page === "customers" && (
            <CustomerPage
              customers={customers}
              loading={loading}
              onRefresh={loadCustomers}
            />
          )}

          {page === "tickets" && (
            <TicketPage
              tickets={filteredTickets}
              totalTickets={tickets.length}
              loading={loading}
              search={ticketSearch}
              setSearch={setTicketSearch}
              statusFilter={ticketStatusFilter}
              setStatusFilter={setTicketStatusFilter}
              priorityFilter={ticketPriorityFilter}
              setPriorityFilter={setTicketPriorityFilter}
              onRefresh={loadTickets}
              onSearch={searchTickets}
              onTicketClick={(ticketId) => {
                setSelectedTicketId(ticketId);
                setPage("ticket-detail");
              }}
              getCustomerName={getCustomerName}
              getDepartmentName={getDepartmentName}
              getCategoryName={getCategoryName}
              getTicketStatus={getTicketStatus}
              getPriority={getPriority}
              formatDate={formatDate}
            />
          )}

          {page === "ticket-detail" && selectedTicketId && (
            <TicketDetailPage
              ticketId={selectedTicketId}
              onBack={() => setPage("tickets")}
              formatDate={formatDate}
            />
          )}

          {page === "create-ticket" && (
            <section className="card form-card">
              <div className="page-header">
                <div>
                  <div className="breadcrumb">
                    Talepler <span>/</span> Yeni Talep
                  </div>

                  <h1>Yeni Talep Oluştur</h1>

                  <p>
                    Müşteri için yeni bir destek talebi oluşturun.
                  </p>
                </div>
              </div>

              <form onSubmit={handleCreateTicket}>
                <div className="form-grid">
                  <div className="form-group customer-search-group">
                    <label>Müşteri</label>

                    <div className="customer-search-wrapper">
                      <input
                        type="text"
                        value={customerSearch}
                        placeholder="Ad, soyad, TCK No veya e-posta ile ara..."
                        onChange={(e) => {
                          setCustomerSearch(e.target.value);
                          setCustomerDropdownOpen(true);

                          if (
                            selectedCustomer &&
                            e.target.value !==
                              `${selectedCustomer.firstName} ${selectedCustomer.lastName}`
                          ) {
                            setTicketForm((current) => ({
                              ...current,
                              customerId: "",
                            }));
                          }
                        }}
                        onFocus={() => {
                          if (customerSearch.trim()) {
                            setCustomerDropdownOpen(true);
                          }
                        }}
                      />

                      {customerDropdownOpen &&
                        customerSearch.trim() && (
                          <div className="customer-search-results">
                            {filteredTicketCustomers.length > 0 ? (
                              filteredTicketCustomers.map((customer) => (
                                <button
                                  type="button"
                                  key={customer.id}
                                  className="customer-search-result"
                                  onClick={() =>
                                    handleCustomerSelect(customer)
                                  }
                                >
                                  <strong>
                                    {customer.firstName}{" "}
                                    {customer.lastName}
                                  </strong>

                                  <span>
                                    TCK No: {customer.tckNo}
                                  </span>

                                  <span>{customer.email}</span>
                                </button>
                              ))
                            ) : (
                              <div className="customer-no-result">
                                Müşteri bulunamadı.
                              </div>
                            )}
                          </div>
                        )}
                    </div>

                    {selectedCustomer && (
                      <div className="selected-customer">
                        <div>
                          <strong>
                            {selectedCustomer.firstName}{" "}
                            {selectedCustomer.lastName}
                          </strong>

                          <span>
                            TCK No: {selectedCustomer.tckNo}
                          </span>

                          <span>{selectedCustomer.email}</span>
                        </div>

                        <button
                          type="button"
                          onClick={() => {
                            setTicketForm((current) => ({
                              ...current,
                              customerId: "",
                            }));

                            setCustomerSearch("");
                            setCustomerDropdownOpen(false);
                          }}
                        >
                          Değiştir
                        </button>
                      </div>
                    )}
                  </div>

                  <div className="form-group">
                    <label>Departman</label>

                    <select
                      value={ticketForm.departmentId}
                      onChange={(e) =>
                        setTicketForm({
                          ...ticketForm,
                          departmentId: e.target.value,
                        })
                      }
                    >
                      <option value="">Departman seçin</option>

                      {departments.map((department) => (
                        <option
                          key={department.id}
                          value={department.id}
                        >
                          {department.name}
                        </option>
                      ))}
                    </select>
                  </div>

                  <div className="form-group">
                    <label>Kategori</label>

                    <select
                      value={ticketForm.categoryId}
                      onChange={(e) =>
                        handleCategoryChange(e.target.value)
                      }
                    >
                      <option value="">Kategori seçin</option>

                      {categories
                        .filter((category) => category.isActive)
                        .map((category) => (
                          <option
                            key={category.id}
                            value={category.id}
                          >
                            {category.name}
                          </option>
                        ))}
                    </select>
                  </div>

                  <div className="form-group">
                    <label>Alt Kategori</label>

                    <select
                      value={ticketForm.subCategoryId}
                      disabled={!ticketForm.categoryId}
                      onChange={(e) =>
                        setTicketForm({
                          ...ticketForm,
                          subCategoryId: e.target.value,
                        })
                      }
                    >
                      <option value="">
                        Alt kategori seçin
                      </option>

                      {subCategories
                        .filter((subCategory) => subCategory.isActive)
                        .map((subCategory) => (
                          <option
                            key={subCategory.id}
                            value={subCategory.id}
                          >
                            {subCategory.name}
                          </option>
                        ))}
                    </select>
                  </div>

                  <div className="form-group">
                    <label>Öncelik</label>

                    <select
                      value={ticketForm.priority}
                      onChange={(e) =>
                        setTicketForm({
                          ...ticketForm,
                          priority: Number(e.target.value),
                        })
                      }
                    >
                      <option value={1}>Düşük</option>
                      <option value={2}>Orta</option>
                      <option value={3}>Yüksek</option>
                      <option value={4}>Kritik</option>
                    </select>
                  </div>

                  <div className="form-group full">
                    <label>Konu</label>

                    <input
                      value={ticketForm.subject}
                      onChange={(e) =>
                        setTicketForm({
                          ...ticketForm,
                          subject: e.target.value,
                        })
                      }
                      placeholder="Talebin konusunu girin"
                    />
                  </div>

                  <div className="form-group full">
                    <label>Açıklama</label>

                    <textarea
                      rows={7}
                      value={ticketForm.description}
                      onChange={(e) =>
                        setTicketForm({
                          ...ticketForm,
                          description: e.target.value,
                        })
                      }
                      placeholder="Talebi ayrıntılı şekilde açıklayın..."
                    />
                  </div>
                </div>

                <div className="form-actions">
                  <button
                    type="submit"
                    className="primary-button"
                    disabled={saving}
                  >
                    {saving
                      ? "Oluşturuluyor..."
                      : "Talep Oluştur"}
                  </button>
                </div>
              </form>
            </section>
          )}
        </main>
      </div>
    </div>
  );
}

function TicketPage({
  tickets,
  totalTickets,
  loading,
  search,
  setSearch,
  statusFilter,
  setStatusFilter,
  priorityFilter,
  setPriorityFilter,
  onRefresh,
  onSearch,
  onTicketClick,
  getCustomerName,
  getDepartmentName,
  getCategoryName,
  getTicketStatus,
  getPriority,
  formatDate,
}: {
  tickets: Ticket[];
  totalTickets: number;
  loading: boolean;
  search: string;
  setSearch: (value: string) => void;
  statusFilter: string;
  setStatusFilter: (value: string) => void;
  priorityFilter: string;
  setPriorityFilter: (value: string) => void;
  onRefresh: () => void;
  onSearch: () => void;
  onTicketClick: (ticketId: string) => void;
  getCustomerName: (id: string) => string;
  getDepartmentName: (id: string) => string;
  getCategoryName: (id: string) => string;
  getTicketStatus: (status: number) => {
    text: string;
    className: string;
  };
  getPriority: (priority: number) => {
    text: string;
    className: string;
  };
  formatDate: (date?: string | null) => string;
}) {
  return (
    <>
      <div className="page-header">
        <div>
          <div className="breadcrumb">
            Talepler <span>/</span> Tüm Talepler
          </div>

          <h1>Talep Yönetimi</h1>

          <p>
            Sistemde kayıtlı destek taleplerini görüntüleyin ve
            filtreleyin.
          </p>
        </div>

        <div className="department-count">
          <strong>{totalTickets}</strong>
          <span>Talep</span>
        </div>
      </div>

      <section className="card">
        <div className="card-header">
          <div>
            <h2>Talepler</h2>

            <p>
              Talep numarası, konu, müşteri veya kategori ile
              arama yapabilirsiniz.
            </p>
          </div>

          <button
            className="secondary-button"
            onClick={onRefresh}
            disabled={loading}
          >
            Yenile
          </button>
        </div>

        <div
          style={{
            display: "grid",
            gridTemplateColumns: "1fr 180px 180px",
            gap: "12px",
            padding: "20px 24px",
          }}
        >
          <div style={{ display: "flex", gap: "8px" }}>
            <input
              style={{
                flex: 1,
                padding: "12px",
                border: "1px solid #dce1e8",
                borderRadius: "8px",
              }}
              placeholder="Talep no, konu, müşteri veya kategori ara..."
              value={search}
              onChange={(e) => setSearch(e.target.value)}
              onKeyDown={(e) => {
                if (e.key === "Enter") {
                  onSearch();
                }
              }}
            />

            <button
              className="primary-button"
              onClick={onSearch}
              disabled={loading}
            >
              Ara
            </button>
          </div>

          <select
            value={statusFilter}
            onChange={(e) => setStatusFilter(e.target.value)}
            style={{
              padding: "12px",
              border: "1px solid #dce1e8",
              borderRadius: "8px",
            }}
          >
            <option value="all">Tüm Durumlar</option>
            <option value="1">Açık</option>
            <option value="2">Devam Ediyor</option>
            <option value="3">Çözüldü</option>
            <option value="4">Kapalı</option>
          </select>

          <select
            value={priorityFilter}
            onChange={(e) => setPriorityFilter(e.target.value)}
            style={{
              padding: "12px",
              border: "1px solid #dce1e8",
              borderRadius: "8px",
            }}
          >
            <option value="all">Tüm Öncelikler</option>
            <option value="1">Düşük</option>
            <option value="2">Orta</option>
            <option value="3">Yüksek</option>
            <option value="4">Kritik</option>
          </select>
        </div>

        {loading ? (
          <div className="loading">
            Talepler yükleniyor...
          </div>
        ) : (
          <div className="table-wrapper">
            <table>
              <thead>
                <tr>
                  <th>Talep No</th>
                  <th>Konu</th>
                  <th>Müşteri</th>
                  <th>Departman</th>
                  <th>Kategori</th>
                  <th>Öncelik</th>
                  <th>Durum</th>
                  <th>SLA</th>
                </tr>
              </thead>

              <tbody>
                {tickets.map((ticket) => {
                  const status = getTicketStatus(ticket.status);
                  const priority = getPriority(ticket.priority);

                  return (
                    <tr
                      key={ticket.id}
                      onClick={() => onTicketClick(ticket.id)}
                      style={{ cursor: "pointer" }}
                    >
                      <td>
                        <span className="code">
                          {ticket.ticketNumber}
                        </span>
                      </td>

                      <td>
                        <strong>{ticket.subject}</strong>

                        <div
                          style={{
                            fontSize: "12px",
                            color: "#7b8491",
                            marginTop: "4px",
                            maxWidth: "280px",
                          }}
                        >
                          {ticket.description}
                        </div>
                      </td>

                      <td>
                        {getCustomerName(ticket.customerId)}
                      </td>

                      <td>
                        {getDepartmentName(ticket.departmentId)}
                      </td>

                      <td>
                        {getCategoryName(ticket.categoryId)}
                      </td>

                      <td>
                        <span
                          className={`priority ${priority.className}`}
                        >
                          {priority.text}
                        </span>
                      </td>

                      <td>
                        <span
                          className={`status ${status.className}`}
                        >
                          {status.text}
                        </span>
                      </td>

                      <td>
                        <div
                          style={{
                            fontSize: "12px",
                            lineHeight: "1.5",
                          }}
                        >
                          <strong>Yanıt:</strong>{" "}
                          {formatDate(ticket.slaResponseDueAt)}
                          <br />

                          <strong>Çözüm:</strong>{" "}
                          {formatDate(ticket.slaResolutionDueAt)}
                        </div>
                      </td>
                    </tr>
                  );
                })}

                {tickets.length === 0 && (
                  <tr>
                    <td
                      colSpan={8}
                      style={{
                        textAlign: "center",
                        padding: "40px",
                      }}
                    >
                      Filtrelere uygun talep bulunamadı.
                    </td>
                  </tr>
                )}
              </tbody>
            </table>
          </div>
        )}
      </section>
    </>
  );
}

function DepartmentPage({
  departments,
  loading,
  onRefresh,
}: {
  departments: Department[];
  loading: boolean;
  onRefresh: () => void;
}) {
  return (
    <>
      <div className="page-header">
        <div>
          <div className="breadcrumb">
            Yönetim <span>/</span> Departmanlar
          </div>

          <h1>Departman Yönetimi</h1>

          <p>
            Destek departmanlarını görüntüleyin ve yönetin.
          </p>
        </div>

        <div className="department-count">
          <strong>{departments.length}</strong>
          <span>Departman</span>
        </div>
      </div>

      <section className="card">
        <div className="card-header">
          <div>
            <h2>Departmanlar</h2>

            <p>
              Sistemde kayıtlı destek departmanları.
            </p>
          </div>

          <button
            className="secondary-button"
            onClick={onRefresh}
            disabled={loading}
          >
            Yenile
          </button>
        </div>

        {loading ? (
          <div className="loading">
            Departmanlar yükleniyor...
          </div>
        ) : (
          <div className="table-wrapper">
            <table>
              <thead>
                <tr>
                  <th>Kod</th>
                  <th>Departman</th>
                  <th>Açıklama</th>
                  <th>Durum</th>
                </tr>
              </thead>

              <tbody>
                {departments.map((department) => (
                  <tr key={department.id}>
                    <td>
                      <span className="code">
                        {department.code}
                      </span>
                    </td>

                    <td>
                      <strong>{department.name}</strong>
                    </td>

                    <td className="description">
                      {department.description}
                    </td>

                    <td>
                      <span
                        className={
                          department.status === 1
                            ? "status active"
                            : "status passive"
                        }
                      >
                        {department.status === 1
                          ? "Aktif"
                          : "Pasif"}
                      </span>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        )}
      </section>
    </>
  );
}

function CustomerPage({
  customers,
  loading,
  onRefresh,
}: {
  customers: Customer[];
  loading: boolean;
  onRefresh: () => void;
}) {
  const [search, setSearch] = useState("");

  const filteredCustomers = customers.filter((customer) => {
    const text =
      `${customer.firstName} ${customer.lastName} ${customer.tckNo} ${customer.email}`.toLowerCase();

    return text.includes(search.toLowerCase());
  });

  const formatBirthDate = (date: string) => {
    if (!date || date.startsWith("0001-01-01")) {
      return "-";
    }

    const parsedDate = new Date(date);

    if (Number.isNaN(parsedDate.getTime())) {
      return "-";
    }

    return parsedDate.toLocaleDateString("tr-TR");
  };

  return (
    <>
      <div className="page-header">
        <div>
          <div className="breadcrumb">
            Yönetim <span>/</span> Müşteriler
          </div>

          <h1>Müşteri Arama</h1>

          <p>
            Sistemde kayıtlı müşterileri arayın ve inceleyin.
          </p>
        </div>

        <div className="department-count">
          <strong>{customers.length}</strong>
          <span>Müşteri</span>
        </div>
      </div>

      <section className="card">
        <div className="card-header">
          <div>
            <h2>Müşteri Ara</h2>

            <p>
              Ad, soyad, TCK No veya e-posta ile arama
              yapabilirsiniz.
            </p>
          </div>

          <button
            className="secondary-button"
            onClick={onRefresh}
            disabled={loading}
          >
            Yenile
          </button>
        </div>

        <div style={{ padding: "20px 24px" }}>
          <input
            style={{
              width: "100%",
              padding: "12px",
              border: "1px solid #dce1e8",
              borderRadius: "8px",
            }}
            placeholder="Müşteri ara..."
            value={search}
            onChange={(e) => setSearch(e.target.value)}
          />
        </div>

        {loading ? (
          <div className="loading">
            Müşteriler yükleniyor...
          </div>
        ) : (
          <div className="table-wrapper">
            <table>
              <thead>
                <tr>
                  <th>Ad Soyad</th>
                  <th>TCK No</th>
                  <th>E-posta</th>
                  <th>Doğum Tarihi</th>
                  <th>Durum</th>
                </tr>
              </thead>

              <tbody>
                {filteredCustomers.map((customer) => (
                  <tr key={customer.id}>
                    <td>
                      <strong>
                        {customer.firstName}{" "}
                        {customer.lastName}
                      </strong>
                    </td>

                    <td>{customer.tckNo}</td>

                    <td>{customer.email}</td>

                    <td>
                      {formatBirthDate(customer.birthDate)}
                    </td>

                    <td>
                      <span
                        className={
                          customer.status === 1
                            ? "status active"
                            : "status passive"
                        }
                      >
                        {customer.status === 1
                          ? "Aktif"
                          : "Pasif"}
                      </span>
                    </td>
                  </tr>
                ))}

                {filteredCustomers.length === 0 && (
                  <tr>
                    <td
                      colSpan={5}
                      style={{
                        textAlign: "center",
                        padding: "30px",
                      }}
                    >
                      Müşteri bulunamadı.
                    </td>
                  </tr>
                )}
              </tbody>
            </table>
          </div>
        )}
      </section>
    </>
  );
}

function TicketDetailPage({
  ticketId,
  onBack,
  formatDate,
}: {
  ticketId: string;
  onBack: () => void;
  formatDate: (date?: string | null) => string;
}) {
  const [ticket, setTicket] = useState<Ticket | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  const [comments, setComments] = useState<Comment[]>([]);
  const [commentsLoading, setCommentsLoading] = useState(false);

  const [commentContent, setCommentContent] = useState("");
  const [commentAuthor, setCommentAuthor] = useState("İklim");
  const [commentType, setCommentType] = useState(1);
  const [commentSaving, setCommentSaving] = useState(false);

  const loadTicket = async () => {
    try {
      setLoading(true);
      setError("");

      const response = await fetch(
        `${API_BASE}/Tickets/${ticketId}`
      );

      if (!response.ok) {
        throw new Error("Talep bilgileri alınamadı.");
      }

      const data = await response.json();

      setTicket({
        id: data.id ?? data.ticketId,
        ticketNumber: data.ticketNumber,
        customerId: data.customerId,
        departmentId: data.departmentId,
        assignedUserId: data.assignedUserId ?? null,
        categoryId: data.categoryId,
        subCategoryId: data.subCategoryId ?? null,
        subject: data.subject,
        description: data.description ?? "",

        status:
          typeof data.status === "number"
            ? data.status
            : data.status === "Open"
            ? 1
            : data.status === "InProgress"
            ? 2
            : data.status === "Resolved"
            ? 3
            : data.status === "Closed"
            ? 4
            : 1,

        priority:
          typeof data.priority === "number"
            ? data.priority
            : data.priority === "Low"
            ? 1
            : data.priority === "Medium"
            ? 2
            : data.priority === "High"
            ? 3
            : data.priority === "Critical"
            ? 4
            : 1,

        slaStartedAt: data.slaStartedAt ?? null,
        slaResponseDueAt: data.slaResponseDueAt ?? null,
        slaResolutionDueAt: data.slaResolutionDueAt ?? null,
        isSlaPaused: data.isSlaPaused ?? false,
        slaPausedAt: data.slaPausedAt ?? null,
        totalSlaPausedDuration:
          data.totalSlaPausedDuration ?? null,
      });
    } catch (err) {
      setError(
        err instanceof Error
          ? err.message
          : "Talep bilgileri alınamadı."
      );
    } finally {
      setLoading(false);
    }
  };

  const loadComments = async () => {
    try {
      setCommentsLoading(true);
      setError("");

      const response = await fetch(
        `${API_BASE}/Tickets/${ticketId}/comments`
      );

      if (!response.ok) {
        throw new Error("Yorumlar alınamadı.");
      }

      const data = await response.json();

      setComments(Array.isArray(data) ? data : []);
    } catch (err) {
      setError(
        err instanceof Error
          ? err.message
          : "Yorumlar alınamadı."
      );
    } finally {
      setCommentsLoading(false);
    }
  };

  useEffect(() => {
    loadTicket();
    loadComments();
  }, [ticketId]);

  const handleAddComment = async (
    event: React.FormEvent<HTMLFormElement>
  ) => {
    event.preventDefault();

    setError("");

    if (!commentContent.trim()) {
      setError("Lütfen yorum metni girin.");
      return;
    }

    if (!commentAuthor.trim()) {
      setError("Lütfen yorum sahibini girin.");
      return;
    }

    try {
      setCommentSaving(true);

      const response = await fetch(
        `${API_BASE}/Tickets/${ticketId}/comments`,
        {
          method: "POST",
          headers: {
            "Content-Type": "application/json",
          },
          body: JSON.stringify({
            content: commentContent.trim(),
            author: commentAuthor.trim(),
            type: commentType,
          }),
        }
      );

      if (!response.ok) {
        const message = await response.text();

        throw new Error(
          message || "Yorum eklenemedi."
        );
      }

      setCommentContent("");

      await loadComments();
    } catch (err) {
      setError(
        err instanceof Error
          ? err.message
          : "Yorum eklenirken bir hata oluştu."
      );
    } finally {
      setCommentSaving(false);
    }
  };

  const getStatusText = (status: number) => {
    switch (status) {
      case 1:
        return "Açık";
      case 2:
        return "Devam Ediyor";
      case 3:
        return "Çözüldü";
      case 4:
        return "Kapalı";
      default:
        return "Bilinmiyor";
    }
  };

  const getPriorityText = (priority: number) => {
    switch (priority) {
      case 1:
        return "Düşük";
      case 2:
        return "Orta";
      case 3:
        return "Yüksek";
      case 4:
        return "Kritik";
      default:
        return "Bilinmiyor";
    }
  };

  if (loading) {
    return (
      <section className="card">
        <div className="loading">
          Talep bilgileri yükleniyor...
        </div>
      </section>
    );
  }

  if (error && !ticket) {
    return (
      <section className="card">
        <div className="error">
          <strong>Hata:</strong> {error}
        </div>

        <button
          className="secondary-button"
          onClick={onBack}
        >
          Taleplere Dön
        </button>
      </section>
    );
  }

  if (!ticket) {
    return (
      <section className="card">
        <div className="loading">
          Talep bulunamadı.
        </div>

        <button
          className="secondary-button"
          onClick={onBack}
        >
          Taleplere Dön
        </button>
      </section>
    );
  }

  return (
    <>
      <div className="page-header">
        <div>
          <div className="breadcrumb">
            Talepler <span>/</span> {ticket.ticketNumber}
          </div>

          <h1>{ticket.subject}</h1>

          <p>
            Talep detaylarını ve işlem bilgilerini görüntüleyin.
          </p>
        </div>

        <button
          className="secondary-button"
          onClick={onBack}
        >
          ← Taleplere Dön
        </button>
      </div>

      {error && (
        <div
          className="error"
          style={{ marginBottom: "20px" }}
        >
          <strong>Hata:</strong> {error}
        </div>
      )}

      <section className="card">
        <div className="card-header">
          <div>
            <h2>Talep Bilgileri</h2>

            <p>{ticket.ticketNumber}</p>
          </div>

          <div
            style={{
              display: "flex",
              gap: "8px",
            }}
          >
            <span className="status active">
              {getStatusText(ticket.status)}
            </span>

            <span className="priority medium">
              {getPriorityText(ticket.priority)}
            </span>
          </div>
        </div>

        <div style={{ padding: "24px" }}>
          <div
            style={{
              display: "grid",
              gridTemplateColumns:
                "repeat(2, minmax(0, 1fr))",
              gap: "20px",
            }}
          >
            <div>
              <strong>Talep No</strong>

              <div style={{ marginTop: "6px" }}>
                {ticket.ticketNumber}
              </div>
            </div>

            <div>
              <strong>Talep ID</strong>

              <div
                style={{
                  marginTop: "6px",
                  wordBreak: "break-all",
                }}
              >
                {ticket.id}
              </div>
            </div>

            <div>
              <strong>Müşteri ID</strong>

              <div
                style={{
                  marginTop: "6px",
                  wordBreak: "break-all",
                }}
              >
                {ticket.customerId}
              </div>
            </div>

            <div>
              <strong>Departman ID</strong>

              <div
                style={{
                  marginTop: "6px",
                  wordBreak: "break-all",
                }}
              >
                {ticket.departmentId}
              </div>
            </div>

            <div>
              <strong>Kategori ID</strong>

              <div
                style={{
                  marginTop: "6px",
                  wordBreak: "break-all",
                }}
              >
                {ticket.categoryId}
              </div>
            </div>

            <div>
              <strong>Öncelik</strong>

              <div style={{ marginTop: "6px" }}>
                {getPriorityText(ticket.priority)}
              </div>
            </div>
          </div>

          <div
            style={{
              marginTop: "28px",
              paddingTop: "24px",
              borderTop: "1px solid #e5e7eb",
            }}
          >
            <strong>Konu</strong>

            <div
              style={{
                marginTop: "8px",
                fontSize: "18px",
              }}
            >
              {ticket.subject}
            </div>
          </div>

          <div
            style={{
              marginTop: "24px",
              paddingTop: "24px",
              borderTop: "1px solid #e5e7eb",
            }}
          >
            <strong>Açıklama</strong>

            <div
              style={{
                marginTop: "8px",
                lineHeight: "1.7",
                whiteSpace: "pre-wrap",
              }}
            >
              {ticket.description ||
                "Açıklama bulunmuyor."}
            </div>
          </div>

          <div
            style={{
              marginTop: "24px",
              paddingTop: "24px",
              borderTop: "1px solid #e5e7eb",
            }}
          >
            <strong>SLA Bilgileri</strong>

            <div
              style={{
                marginTop: "12px",
                display: "grid",
                gridTemplateColumns:
                  "repeat(2, minmax(0, 1fr))",
                gap: "16px",
              }}
            >
              <div>
                <small>SLA Başlangıcı</small>

                <div style={{ marginTop: "4px" }}>
                  {formatDate(ticket.slaStartedAt)}
                </div>
              </div>

              <div>
                <small>Yanıt Son Tarihi</small>

                <div style={{ marginTop: "4px" }}>
                  {formatDate(ticket.slaResponseDueAt)}
                </div>
              </div>

              <div>
                <small>Çözüm Son Tarihi</small>

                <div style={{ marginTop: "4px" }}>
                  {formatDate(ticket.slaResolutionDueAt)}
                </div>
              </div>

              <div>
                <small>SLA Durumu</small>

                <div style={{ marginTop: "4px" }}>
                  {ticket.isSlaPaused
                    ? "Durduruldu"
                    : "Aktif"}
                </div>
              </div>
            </div>
          </div>
        </div>
      </section>

      <section
        className="card"
        style={{ marginTop: "20px" }}
      >
        <div className="card-header">
          <div>
            <h2>Yorumlar</h2>

            <p>
              Bu talep üzerinde yapılan müşteri ve destek ekibi
              yorumları.
            </p>
          </div>

          <button
            className="secondary-button"
            onClick={loadComments}
            disabled={commentsLoading}
          >
            {commentsLoading
              ? "Yükleniyor..."
              : "Yenile"}
          </button>
        </div>

        <form
          onSubmit={handleAddComment}
          style={{
            padding: "20px 24px",
            borderBottom: "1px solid #e5e7eb",
          }}
        >
          <div
            style={{
              display: "grid",
              gridTemplateColumns:
                "1fr 180px",
              gap: "12px",
              marginBottom: "12px",
            }}
          >
            <div>
              <label
                style={{
                  display: "block",
                  marginBottom: "6px",
                  fontWeight: 600,
                }}
              >
                Yorum Sahibi
              </label>

              <input
                type="text"
                value={commentAuthor}
                onChange={(e) =>
                  setCommentAuthor(e.target.value)
                }
                placeholder="Yorum sahibi"
                style={{
                  width: "100%",
                  padding: "12px",
                  border: "1px solid #dce1e8",
                  borderRadius: "8px",
                }}
              />
            </div>

            <div>
              <label
                style={{
                  display: "block",
                  marginBottom: "6px",
                  fontWeight: 600,
                }}
              >
                Yorum Türü
              </label>

              <select
                value={commentType}
                onChange={(e) =>
                  setCommentType(Number(e.target.value))
                }
                style={{
                  width: "100%",
                  padding: "12px",
                  border: "1px solid #dce1e8",
                  borderRadius: "8px",
                }}
              >
                <option value={1}>
                  Genel Yorum
                </option>

                <option value={2}>
                  İç Not
                </option>
              </select>
            </div>
          </div>

          <label
            style={{
              display: "block",
              marginBottom: "6px",
              fontWeight: 600,
            }}
          >
            Yorum
          </label>

          <textarea
            rows={4}
            value={commentContent}
            onChange={(e) =>
              setCommentContent(e.target.value)
            }
            placeholder="Talep hakkında yorumunuzu yazın..."
            style={{
              width: "100%",
              padding: "12px",
              border: "1px solid #dce1e8",
              borderRadius: "8px",
              resize: "vertical",
              boxSizing: "border-box",
            }}
          />

          <div
            style={{
              marginTop: "12px",
              display: "flex",
              justifyContent: "flex-end",
            }}
          >
            <button
              type="submit"
              className="primary-button"
              disabled={commentSaving}
            >
              {commentSaving
                ? "Gönderiliyor..."
                : "Yorum Ekle"}
            </button>
          </div>
        </form>

        {commentsLoading ? (
          <div className="loading">
            Yorumlar yükleniyor...
          </div>
        ) : comments.length === 0 ? (
          <div
            style={{
              padding: "30px 24px",
              color: "#7b8491",
              textAlign: "center",
            }}
          >
            Bu talep için henüz yorum bulunmuyor.
          </div>
        ) : (
          <div
            style={{
              padding: "0 24px 24px",
            }}
          >
            {comments.map((comment, index) => (
              <div
                key={
                  comment.id ??
                  comment.commentId ??
                  `${comment.createdAt}-${index}`
                }
                style={{
                  padding: "18px 0",
                  borderBottom:
                    "1px solid #e5e7eb",
                }}
              >
                <div
                  style={{
                    display: "flex",
                    justifyContent:
                      "space-between",
                    gap: "20px",
                  }}
                >
                  <strong>
                    {comment.author ??
                      "Bilinmeyen kullanıcı"}
                  </strong>

                  <span
                    style={{
                      fontSize: "12px",
                      color: "#7b8491",
                    }}
                  >
                    {formatDate(
                      comment.createdAt ??
                        comment.occurredOn ??
                        comment.date
                    )}
                  </span>
                </div>

                <div
                  style={{
                    marginTop: "10px",
                    lineHeight: "1.6",
                    whiteSpace: "pre-wrap",
                  }}
                >
                  {comment.content ??
                    comment.text ??
                    ""}
                </div>
              </div>
            ))}
          </div>
        )}
      </section>
    </>
  );
}

export default App;