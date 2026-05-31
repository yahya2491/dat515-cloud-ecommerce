"use client";

import { useEffect, useState } from "react";
import Link from "next/link";
import { getAllProducts } from "../utils/requests"; // make sure this exists
import styles from "./page.module.css";
import productImages from "../utils/productImages";


export default function HomePage() {
  const [products, setProducts] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  useEffect(() => {
    async function fetchProducts() {
      try {
        const data = await getAllProducts();
        setProducts(data.slice(0, 3)); // show only first 3 for featured section
      } catch (err) {
        console.error(err);
        setError("Failed to load products.");
      } finally {
        setLoading(false);
      }
    }
    fetchProducts();
  }, []);

  return (
    <main className={styles.main}>
      {/* Hero Section */}
      <section className={styles.hero}>
        <h2>Fresh Groceries Delivered to Your Door 🍎🥦</h2>
        <p>Shop the freshest fruits, vegetables, and daily essentials online.</p>
        <Link href="/products">
          <button className={styles.ctaBtn}>Shop Now 🛒</button>
        </Link>
      </section>

      {/* Featured Products */}
      <section className={styles.productsSection}>
        <h3>Featured Groceries</h3>

        {loading && <p>Loading products...</p>}
        {error && <p style={{ color: "red" }}>{error}</p>}

        <div className={styles.productsGrid}>
          {products.map((product) => (
            <div key={product.id} className={styles.productCard}>
              <img
                src={
                  productImages[product.name] 
                }
                alt={product.name}
                className={styles.productImage}
              />
              <h4>{product.name}</h4>
              <p className={styles.price}>${product.price.toFixed(2)}</p>
            </div>
          ))}
        </div>
      </section>
    </main>
  );
}
