"use client";

import { useEffect, useState, useContext } from "react";
import { getAllProducts, addItemToUserCart } from "../../utils/requests";
import { UserContext } from "../../context/UserContext";
import styles from "../page.module.css";
import productImages from "../../utils/productImages";


export default function ProductsPage() {
  const [products, setProducts] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");
  const [message, setMessage] = useState("");
  const { user } = useContext(UserContext); // get logged in user

  useEffect(() => {
    async function fetchProducts() {
      try {
        const data = await getAllProducts();
        setProducts(data);
      } catch (err) {
        console.error(err);
        setError("Failed to fetch products");
      } finally {
        setLoading(false);
      }
    }

    fetchProducts();
  }, []);

  async function handleAddToCart(productId) {
    if (!user || !user.id) {
      setMessage("Please log in to add items to your cart.");
      return;
    }

    try {
      await addItemToUserCart(user.id, productId, 1);
      setMessage("Added to cart!");
      setTimeout(() => setMessage(""), 2000); // Clear message after 2s
    } catch (err) {
      console.error(err);
      setMessage("Failed to add item to cart.");
    }
  }

  if (loading) return <p style={{ textAlign: "center" }}>Loading products...</p>;
  if (error) return <p style={{ color: "red", textAlign: "center" }}>{error}</p>;

  return (
    <main className={styles.productsWrapper}>
      <h1>🛒 Our Products</h1>
      <p>Browse our wide selection of fresh groceries and essentials.</p>

      {message && (
        <p style={{ textAlign: "center", color: "green", margin: "10px 0" }}>
          {message}
        </p>
      )}

      <div className={styles.productsGrid}>
        {products.map((product) => (
          <div key={product.id} className={styles.productCard}>
            <img
              src={
                productImages[product.name] ||
                "/images/products/placeholder.png"
              }
              alt={product.name}
              className={styles.productImage}
            />
            <h3>{product.name}</h3>
            <p>${product.price?.toFixed(2) ?? "N/A"}</p>
            <p>{product.description}</p>
            <button
              className={styles.addBtn}
              onClick={() => handleAddToCart(product.id)}
            >
              ➕ Add to Cart
            </button>
          </div>
        ))}
      </div>
    </main>
  );
}
