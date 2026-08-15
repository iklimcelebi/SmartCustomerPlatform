import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";

import ErrorMessage from "../components/common/ErrorMessage";

import { getCustomers } from "../services/customerService";
import { getCategories } from "../services/categoryService";
import { getDepartments } from "../services/departmentService";
import { createTicket } from "../services/ticketService";

type Customer = {
  id: string;
  name: string;
  email?: string;
};

type Category = {
  id: string;
  name: string;
};

type Department = {
  id: string;
  name: string;
};

export default function CreateTicketPage() {
  const navigate = useNavigate();

  const [customers, setCustomers] =
    useState<Customer[]>([]);

  const [categories, setCategories] =
    useState<Category[]>([]);

  const [departments, setDepartments] =
    useState<Department[]>([]);

  const [customerId, setCustomerId] = useState("");
  const [departmentId, setDepartmentId] =
    useState("");

  const [categoryId, setCategoryId] =
    useState("");

  const [subject, setSubject] =
    useState("");

  const [description, setDescription] =
    useState("");

  const [priority, setPriority] =
    useState("1");

  const [loading, setLoading] =
    useState(true);

  const [saving, setSaving] =
    useState(false);

  const [error, setError] =
    useState("");

  useEffect(() => {
    const loadData = async () => {
      try {
        setLoading(true);
        setError("");

        const [
          customerData,
          categoryData,
          departmentData,
        ] = await Promise.all([
          getCustomers(),
          getCategories(),
          getDepartments(),
        ]);

        setCustomers(customerData);
        setCategories(categoryData);
        setDepartments(departmentData);
      } catch (err) {
        console.error(err);

        setError(
          err instanceof Error
            ? err.message
            : "Form verileri yüklenemedi."
        );
      } finally {
        setLoading(false);
      }
    };

    loadData();
  }, []);

  const handleSubmit = async () => {
    if (
      !customerId ||
      !departmentId ||
      !categoryId ||
      !subject.trim() ||
      !description.trim()
    ) {
      setError(
        "Lütfen müşteri, departman, kategori, başlık ve açıklama alanlarını doldurun."
      );

      return;
    }

    try {
      setSaving(true);
      setError("");

      const ticketId = await createTicket({
        customerId,
        departmentId,
        categoryId,
        subject: subject.trim(),
        description: description.trim(),
        priority: Number(priority),
      });

      navigate(`/tickets/${ticketId}`);
    } catch (err) {
      console.error(err);

      setError(
        err instanceof Error
          ? err.message
          : "Talep oluşturulamadı."
      );
    } finally {
      setSaving(false);
    }
  };

  if (loading) {
    return (
      <div className="container">
        <div className="card">
          <div className="loading">
            Form verileri yükleniyor...
          </div>
        </div>
      </div>
    );
  }

  return (
    <div className="container">
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

      <ErrorMessage message={error} />

      <div className="card form-card">
        <div className="card-header">
          <div>
            <h2>Talep Bilgileri</h2>

            <p>
              Talep için gerekli bilgileri girin.
            </p>
          </div>
        </div>

        <div className="form-grid">
          <div className="form-group">
            <label>Müşteri</label>

            <select
              value={customerId}
              onChange={(event) =>
                setCustomerId(
                  event.target.value
                )
              }
            >
              <option value="">
                Müşteri seçin
              </option>

              {customers.map((customer) => (
                <option
                  key={customer.id}
                  value={customer.id}
                >
                  {customer.name}
                  {customer.email
                    ? ` - ${customer.email}`
                    : ""}
                </option>
              ))}
            </select>
          </div>

          <div className="form-group">
            <label>Departman</label>

            <select
              value={departmentId}
              onChange={(event) =>
                setDepartmentId(
                  event.target.value
                )
              }
            >
              <option value="">
                Departman seçin
              </option>

              {departments.map(
                (department) => (
                  <option
                    key={department.id}
                    value={department.id}
                  >
                    {department.name}
                  </option>
                )
              )}
            </select>
          </div>

          <div className="form-group">
            <label>Kategori</label>

            <select
              value={categoryId}
              onChange={(event) =>
                setCategoryId(
                  event.target.value
                )
              }
            >
              <option value="">
                Kategori seçin
              </option>

              {categories.map((category) => (
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
            <label>Başlık</label>

            <input
              value={subject}
              onChange={(event) =>
                setSubject(
                  event.target.value
                )
              }
              placeholder="Talep başlığı"
            />
          </div>

          <div className="form-group">
            <label>Öncelik</label>

            <select
              value={priority}
              onChange={(event) =>
                setPriority(
                  event.target.value
                )
              }
            >
              <option value="4">
                4 - Kritik
              </option>

              <option value="3">
                3 - Yüksek
              </option>

              <option value="2">
                2 - Orta
              </option>

              <option value="1">
                1 - Düşük
              </option>
            </select>
          </div>

          <div className="form-group full">
            <label>Açıklama</label>

            <textarea
              rows={7}
              value={description}
              onChange={(event) =>
                setDescription(
                  event.target.value
                )
              }
              placeholder="Talebin detaylarını yazın..."
            />
          </div>
        </div>

        <div className="form-actions">
          <button
            type="button"
            className="secondary-button"
            onClick={() =>
              navigate("/tickets")
            }
          >
            Vazgeç
          </button>

          <button
            type="button"
            className="primary-button"
            disabled={saving}
            onClick={handleSubmit}
          >
            {saving
              ? "Oluşturuluyor..."
              : "Talep Oluştur"}
          </button>
        </div>
      </div>
    </div>
  );
}