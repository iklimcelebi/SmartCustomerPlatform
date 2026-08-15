import { useEffect, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";

import Loading from "../components/common/Loading";
import ErrorMessage from "../components/common/ErrorMessage";

import {
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
  name: string;
  description?: string;
  subCategories?: SubCategory[];
  subcategories?: SubCategory[];
};

export default function CategoryDetailPage() {
  const { categoryId } = useParams();
  const navigate = useNavigate();

  const [category, setCategory] = useState<Category | null>(null);
  const [subCategories, setSubCategories] = useState<SubCategory[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  useEffect(() => {
    const load = async () => {
      if (!categoryId) return;

      try {
        setLoading(true);
        setError("");

        const categories = await getCategories();

        const found = categories.find(
          (item) => item.id === categoryId
        );

        if (!found) {
          setError("Kategori bulunamadı.");
          return;
        }

        setCategory(found);

        const subs = await getSubcategories(categoryId);
        setSubCategories(subs);
      } catch (err) {
        setError(
          err instanceof Error
            ? err.message
            : "Kategori detayları yüklenemedi."
        );
      } finally {
        setLoading(false);
      }
    };

    load();
  }, [categoryId]);

  if (loading) {
    return (
      <div className="category-page">
        <div className="container">
          <Loading message="Kategori yükleniyor..." />
        </div>
      </div>
    );
  }

  if (!category) {
    return (
      <div className="category-page">
        <div className="container">
          <ErrorMessage
            message={error || "Kategori bulunamadı."}
          />

          <button
            className="secondary-button"
            onClick={() => navigate("/categories")}
          >
            ← Kategorilere Dön
          </button>
        </div>
      </div>
    );
  }

  return (
    <div className="category-page">
      <div className="container">

        <div className="global-back-bar">
          <button
            className="back-button"
            onClick={() => navigate("/categories")}
          >
            ← Kategoriler
          </button>

          <span>
            Yönetim / Kategoriler / {category.name}
          </span>
        </div>

        <div className="page-header">
          <div>
            <div className="breadcrumb">
              Yönetim / Kategoriler
            </div>

            <h1>{category.name}</h1>

            {category.description && (
              <p>{category.description}</p>
            )}
          </div>
        </div>

        <ErrorMessage message={error} />

        <div className="dashboard-module-grid">

          <div className="card dashboard-module-card">
            <span className="dashboard-module-label">
              ALT KATEGORİ
            </span>

            <strong>
              {subCategories.length}
            </strong>

            <span>
              Bu kategoriye bağlı alt kategori
            </span>
          </div>

          <div className="card dashboard-module-card">
            <span className="dashboard-module-label">
              DURUM
            </span>

            <strong>
              Aktif
            </strong>

            <span>
              Kategori kullanılabilir durumda
            </span>
          </div>

        </div>

        <div className="card">

          <div className="card-header">
            <div>
              <h2>Alt Kategoriler</h2>

              <p>
                {category.name} kategorisine bağlı alt kategoriler.
              </p>
            </div>
          </div>

          <div className="category-list">

            {subCategories.length === 0 ? (
              <div className="category-empty-card">
                <h3>Alt kategori bulunmuyor</h3>

                <p>
                  Bu kategori için henüz alt kategori
                  oluşturulmamış.
                </p>
              </div>
            ) : (
              subCategories.map((subCategory) => (
                <div
                  className="category-item"
                  key={subCategory.id}
                >
                  <div className="category-main">

                    <div>
                      <strong>
                        {subCategory.name}
                      </strong>

                      {subCategory.description && (
                        <p>
                          {subCategory.description}
                        </p>
                      )}
                    </div>

                    <span className="status active">
                      Aktif
                    </span>

                  </div>
                </div>
              ))
            )}

          </div>

        </div>

      </div>
    </div>
  );
}