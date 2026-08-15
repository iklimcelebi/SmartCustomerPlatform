import { useEffect, useMemo, useState } from "react";
import type { FormEvent } from "react";
import { useNavigate } from "react-router-dom";

import Loading from "../components/common/Loading";
import ErrorMessage from "../components/common/ErrorMessage";

import {
  createCategory,
  getCategories,
  getSubcategories,
} from "../services/categoryService";

type SubCategory = {
  id: string;
  name: string;
  description?: string;
};

type Category = {
  id: string;
  code?: string;
  name: string;
  description?: string;
  isActive?: boolean;
};

type CategoryWithSubcategories = Category & {
  subCategories: SubCategory[];
};

export default function CategoryPage() {
  const navigate = useNavigate();

  const [categories, setCategories] = useState<
    CategoryWithSubcategories[]
  >([]);

  const [code, setCode] = useState("");
  const [name, setName] = useState("");
  const [description, setDescription] = useState("");
  const [isActive, setIsActive] = useState(true);

  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState("");

  /*
   * Kategorileri ve her kategorinin alt kategorilerini yükler.
   */
  const loadCategories = async () => {
    try {
      setLoading(true);
      setError("");

      // Önce ana kategorileri al
      const categoryData = await getCategories();

      // Her kategori için alt kategorileri ayrı endpoint'ten al
      const categoriesWithSubcategories =
        await Promise.all(
          categoryData.map(async (category) => {
            try {
              const subCategories =
                await getSubcategories(category.id);

              return {
                ...category,
                subCategories,
              };
            } catch {
              // Bir kategorinin alt kategorileri alınamazsa
              // o kategoriyi yine de göstermeye devam et.
              return {
                ...category,
                subCategories: [],
              };
            }
          })
        );

      setCategories(categoriesWithSubcategories);
    } catch (err) {
      setError(
        err instanceof Error
          ? err.message
          : "Kategoriler yüklenemedi."
      );
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    loadCategories();
  }, []);

  /*
   * Tüm alt kategorilerin toplamı
   */
  const totalSubCategories = useMemo(() => {
    return categories.reduce(
      (total, category) =>
        total + category.subCategories.length,
      0
    );
  }, [categories]);

  /*
   * Aktif kategori sayısı
   */
  const activeCategoryCount = useMemo(() => {
    return categories.filter(
      (category) => category.isActive !== false
    ).length;
  }, [categories]);

  /*
   * Yeni kategori oluştur
   */
  const handleCreate = async (event: FormEvent) => {
    event.preventDefault();

    if (!code.trim() || !name.trim()) {
      setError(
        "Kategori kodu ve kategori adı zorunludur."
      );
      return;
    }

    try {
      setSaving(true);
      setError("");

      await createCategory({
        code: code.trim(),
        name: name.trim(),
        description: description.trim(),
        isActive,
      });

      setCode("");
      setName("");
      setDescription("");
      setIsActive(true);

      // Yeni kategori oluşturulduktan sonra
      // listeyi ve alt kategori sayılarını yenile
      await loadCategories();
    } catch (err) {
      setError(
        err instanceof Error
          ? err.message
          : "Kategori oluşturulamadı."
      );
    } finally {
      setSaving(false);
    }
  };

  /*
   * Formu temizle
   */
  const clearForm = () => {
    setCode("");
    setName("");
    setDescription("");
    setIsActive(true);
    setError("");
  };

  return (
    <div className="page">
      <div className="container">

        {/* =========================
            PAGE HEADER
        ========================= */}

        <div className="page-header">
          <div>
            <div className="breadcrumb">
              Yönetim / Kategoriler
            </div>

            <h1>Kategori Yönetimi</h1>

            <p>
              Destek taleplerinin kategori yapısını yönetin.
            </p>
          </div>

          {/* SUMMARY */}

          <div className="category-summary">

            <div className="category-summary-item">
              <strong>{categories.length}</strong>
              <span>Kategori</span>
            </div>

            <div className="category-summary-divider" />

            <div className="category-summary-item">
              <strong>{totalSubCategories}</strong>
              <span>Alt Kategori</span>
            </div>

            <div className="category-summary-divider" />

            <div className="category-summary-item">
              <strong>{activeCategoryCount}</strong>
              <span>Aktif</span>
            </div>

          </div>
        </div>

        <ErrorMessage message={error} />

        {/* =========================
            CREATE CATEGORY
        ========================= */}

        <div className="card category-create-card">

          <div className="category-create-header">

            <div className="category-create-icon">
              +
            </div>

            <div>
              <h2>Yeni Kategori</h2>

              <p>
                Sisteme yeni bir destek kategorisi ekleyin.
              </p>
            </div>

          </div>

          <form
            className="category-create-form"
            onSubmit={handleCreate}
          >

            {/* CATEGORY CODE */}

            <div className="form-group">
              <label htmlFor="category-code">
                Kategori Kodu
              </label>

              <input
                id="category-code"
                value={code}
                onChange={(event) =>
                  setCode(event.target.value)
                }
                placeholder="Örn. TECH"
                maxLength={50}
              />
            </div>

            {/* CATEGORY NAME */}

            <div className="form-group">
              <label htmlFor="category-name">
                Kategori Adı
              </label>

              <input
                id="category-name"
                value={name}
                onChange={(event) =>
                  setName(event.target.value)
                }
                placeholder="Örn. Teknik Destek"
                maxLength={100}
              />
            </div>

            {/* DESCRIPTION */}

            <div className="form-group">
              <label htmlFor="category-description">
                Açıklama
              </label>

              <input
                id="category-description"
                value={description}
                onChange={(event) =>
                  setDescription(event.target.value)
                }
                placeholder="Kategori hakkında kısa açıklama"
                maxLength={500}
              />
            </div>

            {/* ACTIVE / PASSIVE */}

            <div className="form-group category-status-form-group">

              <label htmlFor="category-status">
                Durum
              </label>

              <label
                className="switch-field"
                htmlFor="category-status"
              >

                <input
                  id="category-status"
                  type="checkbox"
                  checked={isActive}
                  onChange={(event) =>
                    setIsActive(event.target.checked)
                  }
                />

                <span className="switch" />

                <span className="switch-text">
                  {isActive ? "Aktif" : "Pasif"}
                </span>

              </label>

            </div>

            {/* ACTIONS */}

            <div className="form-actions">

              <button
                type="button"
                className="secondary-button"
                onClick={clearForm}
                disabled={saving}
              >
                Temizle
              </button>

              <button
                type="submit"
                className="primary-button"
                disabled={
                  !code.trim() ||
                  !name.trim() ||
                  saving
                }
              >
                {saving
                  ? "Oluşturuluyor..."
                  : "Kategori Oluştur"}
              </button>

            </div>

          </form>
        </div>

        {/* =========================
            CATEGORY LIST
        ========================= */}

        {loading ? (

          <div className="card card-content">
            <Loading message="Kategoriler yükleniyor..." />
          </div>

        ) : categories.length === 0 ? (

          <div className="card empty-state">

            <div className="empty-state-icon">
              +
            </div>

            <h3>
              Henüz kategori bulunmuyor
            </h3>

            <p>
              İlk kategorinizi yukarıdaki formu kullanarak
              oluşturabilirsiniz.
            </p>

          </div>

        ) : (

          <div className="category-list-container">

            {categories.map((category) => {

              /*
               * Artık alt kategoriler doğrudan
               * category.subCategories içerisinde.
               */
              const subCategories =
                category.subCategories;

              const categoryIsActive =
                category.isActive !== false;

              return (
                <div
                  className="category-item-card"
                  key={category.id}
                >

                  {/* =========================
                      CATEGORY HEADER
                  ========================= */}

                  <div className="category-item-header">

                    <div className="category-item-title">

                      <div className="category-name-row">

                        <h3>
                          {category.name}
                        </h3>

                        {category.code && (
                          <span className="category-code">
                            {category.code}
                          </span>
                        )}

                      </div>

                      {category.description && (
                        <p>
                          {category.description}
                        </p>
                      )}

                    </div>

                    {/* ALT KATEGORİ SAYISI */}

                    <div className="category-item-count">

                      <strong>
                        {subCategories.length}
                      </strong>

                      <span>
                        Alt Kategori
                      </span>

                    </div>

                  </div>

                  {/* =========================
                      SUBCATEGORIES
                  ========================= */}

                  {subCategories.length > 0 && (

                    <div className="category-subcategories">

                      <div className="category-subcategories-title">
                        Alt Kategoriler
                      </div>

                      <div className="category-subcategories-list">

                        {subCategories.map(
                          (subCategory) => (

                            <div
                              className="category-subcategory"
                              key={subCategory.id}
                            >

                              <div className="subcategory-main">

                                <span className="subcategory-marker">
                                  •
                                </span>

                                <div>

                                  <strong>
                                    {subCategory.name}
                                  </strong>

                                  {subCategory.description && (
                                    <span>
                                      {subCategory.description}
                                    </span>
                                  )}

                                </div>

                              </div>

                            </div>

                          )
                        )}

                      </div>

                    </div>

                  )}

                  {/* =========================
                      FOOTER
                  ========================= */}

                  <div className="category-item-footer">

                    <div className="category-status">

                      <span
                        className={
                          categoryIsActive
                            ? "category-status-dot active"
                            : "category-status-dot passive"
                        }
                      />

                      <span>
                        {categoryIsActive
                          ? "Aktif kategori"
                          : "Pasif kategori"}
                      </span>

                    </div>

                    <button
                      type="button"
                      className="category-view-button"
                      onClick={() =>
                        navigate(
                          `/categories/${category.id}`
                        )
                      }
                    >
                      Kategoriyi Gör

                      <span>
                        →
                      </span>

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