import { useEffect, useState } from "react";

import Loading from "../components/common/Loading";
import ErrorMessage from "../components/common/ErrorMessage";

import {
getDepartments,
createDepartment,
updateDepartment,
deleteDepartment,
} from "../services/departmentService";

import type {
Department,
DepartmentCreateRequest,
DepartmentUpdateRequest,
} from "../types";

export default function DepartmentPage() {
const [departments, setDepartments] = useState<Department[]>([]);

const [name, setName] = useState("");
const [code, setCode] = useState("");
const [description, setDescription] = useState("");

const [editingId, setEditingId] = useState<string | null>(null);

const [loading, setLoading] = useState(true);
const [saving, setSaving] = useState(false);
const [error, setError] = useState("");

const loadDepartments = async () => {
try {
setLoading(true);
setError("");
  const data = await getDepartments();
  setDepartments(data);
} catch (err) {
  setError(
    err instanceof Error
      ? err.message
      : "Departmanlar yüklenemedi."
  );
} finally {
  setLoading(false);
}
};

useEffect(() => {
loadDepartments();
}, []);

const clearForm = () => {
setName("");
setCode("");
setDescription("");
setEditingId(null);
};

const handleSubmit = async () => {
if (!name.trim()) {
setError("Departman adı zorunludur.");
return;
}


if (!code.trim()) {
  setError("Departman kodu zorunludur.");
  return;
}

if (!description.trim()) {
  setError("Departman açıklaması zorunludur.");
  return;
}

try {
  setSaving(true);
  setError("");

  if (editingId) {
    const data: DepartmentUpdateRequest = {
      id: editingId,
      code: code.trim(),
      name: name.trim(),
      description: description.trim(),
      status: 1,
    };

    await updateDepartment(editingId, data);
  } else {
    const data: DepartmentCreateRequest = {
      code: code.trim(),
      name: name.trim(),
      description: description.trim(),
      status: 1,
    };

    await createDepartment(data);
  }

  clearForm();
  await loadDepartments();
} catch (err) {
  setError(
    err instanceof Error
      ? err.message
      : "Departman kaydedilemedi."
  );
} finally {
  setSaving(false);
}


};

const handleEdit = (department: Department) => {
setEditingId(department.id);
setName(department.name);
setCode(department.code || "");
setDescription(department.description || "");

setError("");

window.scrollTo({
  top: 0,
  behavior: "smooth",
});


};

const handleToggleActive = async (
department: Department
) => {
try {
setSaving(true);
setError("");

  const newStatus =
    department.status === 1 ? 2 : 1;

  const data: DepartmentUpdateRequest = {
    id: department.id,
    code: department.code || "",
    name: department.name,
    description: department.description || "",
    status: newStatus,
  };

  await updateDepartment(department.id, data);

  await loadDepartments();
} catch (err) {
  setError(
    err instanceof Error
      ? err.message
      : "Departman durumu güncellenemedi."
  );
} finally {
  setSaving(false);
}


};

const handleDelete = async (
department: Department
) => {
const confirmed = window.confirm(
`"${department.name}" departmanını silmek istediğinize emin misiniz?`
);


if (!confirmed) {
  return;
}

try {
  setSaving(true);
  setError("");

  await deleteDepartment(department.id);

  if (editingId === department.id) {
    clearForm();
  }

  await loadDepartments();
} catch (err) {
  setError(
    err instanceof Error
      ? err.message
      : "Departman silinemedi."
  );
} finally {
  setSaving(false);
}


};

return ( <div className="container"> <div className="page-header"> <div> <div className="breadcrumb">
Yönetim <span>/</span> Departmanlar </div>

      <h1>Departman Yönetimi</h1>

      <p>
        Destek taleplerinde kullanılacak departmanları yönetin.
      </p>
    </div>

    <div className="department-count">
      <strong>{departments.length}</strong>
      <span>Departman</span>
    </div>
  </div>

  <ErrorMessage message={error} />

  <div className="card form-card">
    <div className="card-header">
      <div>
        <h2>
          {editingId
            ? "Departmanı Düzenle"
            : "Yeni Departman"}
        </h2>

        <p>
          {editingId
            ? "Departman bilgilerini güncelleyin."
            : "Yeni bir destek departmanı oluşturun."}
        </p>
      </div>
    </div>

    <div className="form-grid">
      <div className="form-group">
        <label htmlFor="department-name">
          Departman Adı
        </label>

        <input
          id="department-name"
          value={name}
          onChange={(event) =>
            setName(event.target.value)
          }
          placeholder="Örn. Teknik Destek"
          disabled={saving}
        />
      </div>

      <div className="form-group">
        <label htmlFor="department-code">
          Departman Kodu
        </label>

        <input
          id="department-code"
          value={code}
          onChange={(event) =>
            setCode(event.target.value)
          }
          placeholder="Örn. TECH"
          disabled={saving}
        />
      </div>

      <div className="form-group full">
        <label htmlFor="department-description">
          Açıklama
        </label>

        <textarea
          id="department-description"
          rows={4}
          value={description}
          onChange={(event) =>
            setDescription(event.target.value)
          }
          placeholder="Departman açıklaması"
          disabled={saving}
        />
      </div>
    </div>

    <div className="form-actions">
      {editingId && (
        <button
          type="button"
          className="secondary-button"
          onClick={clearForm}
          disabled={saving}
        >
          İptal
        </button>
      )}

      <button
        type="button"
        className="primary-button"
        onClick={handleSubmit}
        disabled={
          !name.trim() ||
          !code.trim() ||
          !description.trim() ||
          saving
        }
      >
        {saving
          ? "Kaydediliyor..."
          : editingId
            ? "Değişiklikleri Kaydet"
            : "Departman Oluştur"}
      </button>
    </div>
  </div>

  <div className="card">
    <div className="card-header">
      <div>
        <h2>Departman Listesi</h2>

        <p>
          Sistemde tanımlı destek departmanları.
        </p>
      </div>
    </div>

    {loading ? (
      <Loading message="Departmanlar yükleniyor..." />
    ) : departments.length === 0 ? (
      <div className="empty">
        Henüz departman bulunmuyor.
      </div>
    ) : (
      <div className="category-list">
        {departments.map((department) => {
          const isActive = department.status === 1;

          return (
            <div
              className="category-item"
              key={department.id}
            >
              <div className="category-main">
                <div>
                  <strong>
                    {department.name}
                  </strong>

                  {department.code && (
                    <span className="code">
                      {department.code}
                    </span>
                  )}

                  {department.description && (
                    <p>
                      {department.description}
                    </p>
                  )}
                </div>

                <span
                  className={
                    isActive
                      ? "status active"
                      : "status passive"
                  }
                >
                  {isActive
                    ? "Aktif"
                    : "Pasif"}
                </span>
              </div>

              <div
                className="form-actions"
                style={{ marginTop: 12 }}
              >
                <button
                  type="button"
                  className="edit-button"
                  onClick={() =>
                    handleEdit(department)
                  }
                  disabled={saving}
                >
                  Düzenle
                </button>

                <button
                  type="button"
                  className="secondary-button"
                  onClick={() =>
                    handleToggleActive(department)
                  }
                  disabled={saving}
                >
                  {isActive
                    ? "Pasif Yap"
                    : "Aktif Yap"}
                </button>

                <button
                  type="button"
                  className="secondary-button"
                  onClick={() =>
                    handleDelete(department)
                  }
                  disabled={saving}
                >
                  Sil
                </button>
              </div>
            </div>
          );
        })}
      </div>
    )}
  </div>
</div>

);
}
