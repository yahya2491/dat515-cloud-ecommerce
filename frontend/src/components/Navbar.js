"use client";

import Link from "next/link";
import { useContext } from "react";
import { UserContext } from "../context/UserContext";
import styles from "../app/page.module.css";

export default function Navbar() {
  const { user, initialized, logout } = useContext(UserContext);

  return (
    <nav className={styles.navbar}>
      <h1 className={styles.logo}>🥬 FreshMart</h1>
      <ul className={styles.navLinks}>
        <li><Link href="/">Home</Link></li>
        <li><Link href="/products">Products</Link></li>
        <li><Link href="/about">About</Link></li>

        {/* Show login/signup when user not logged in */}
        {initialized && !user && (
          <>
            <li><Link href="/login">Login</Link></li>
            <li><Link href="/register">Signup</Link></li>
          </>
        )}

        {/* Show profile, cart, and logout when logged in */}
        {initialized && user && user.id && (
          <>
            <li>
              <Link href="/cart" className={styles.cartLink}>
                 Cart
              </Link>
            </li>
            <li>
              <Link href="/user" className={styles.profileLink}>
                Profile
              </Link>
            </li>
            <li>
              <button className={styles.logoutBtn} onClick={logout}>
                Logout
              </button>
            </li>
          </>
        )}
      </ul>
    </nav>
  );
}
